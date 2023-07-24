// 규칙: 인덱스 기반 렌덤한 접근이 가능하도록 할 것,
// 인덱스는 곧 ID와 동치로서 쓸 것이다.

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : class
{
    public class PooledObject : IEquatable<T>, IEquatable<PooledObject>
    {
        public bool active;
        public T @object;

        public PooledObject(T @object)
        {
            this.active = false;
            this.@object = @object;
        }
        public PooledObject(T @object, bool active)
        {
            this.@object = @object;
            this.active = active;
        }

        public override int GetHashCode()
        {
            return @object.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is T t)
            {
                return Equals(t);
            }
            if (obj is PooledObject pObj)
            {
                return Equals(pObj);
            }
            return false;
        }

        public bool Equals(T obj)
        {
            if (obj is IEquatable<T> equatable)
            {
                return equatable.Equals(this.@object);
            }
            return obj.Equals(this.@object);
        }
        public bool Equals(PooledObject obj)
        {
            return Equals(obj.@object);
        }
    }

    /// <summary><see cref="PooledObject.active"/>=<see langword="false"/>인 오브젝트를 생성합니다.</summary>
    private readonly Func<T> m_instantiate;
    /// <summary><see cref="PooledObject.@object"/>의 자체 active를 설정합니다.</summary>
    private readonly Action<T, bool> m_setActive;
    /// <summary>모든 오브젝트에 대한 리스트입니다.</summary>
    public readonly List<PooledObject> All = new List<PooledObject>();
    /// <summary> 다음 비활성화되어 있는 오브젝트의 인덱스입니다. -1이면 비활성화되어 있는 오브젝트는 없습니다. </summary>
    private int m_nextFObj = -1;

    /// <summary><see cref="ObjectPool{T}"/> 클래스의 새 인스턴스를 초기화합니다.</summary>
    /// <param name="instantiate">활성화 되지 않은 오브젝트를 생성하는 대리자입니다.</param>
    /// <param name="setActive">오브젝트의 활성화를 설정하는 대리자입니다.</param>
    public ObjectPool(Func<T> instantiate, Action<T, bool> setActive)
    {
        this.m_instantiate = instantiate;
        this.m_setActive = setActive;
#if DEBUG
        if (instantiate == null)
        {
            Debug.LogWarning("ObjectPool::.ctor() - [instance field] instantiate is null");
        }
        if (setActive == null)
        {
            Debug.LogWarning("ObjectPool::.ctor() - [instance field] setActive is null");
        }
#endif
    }

    /// <summary> 새로운 오브젝트를 등록합니다.</summary>
    public void Alloc()
    {
        if (m_instantiate == null)
        {
#if DEBUG
            Debug.LogError("ObjectPool::Alloc() - [instance field] instantiate is null");
#endif
            return;
        }
        PooledObject var = new PooledObject(m_instantiate());
        All.Add(var);
        if (m_nextFObj == -1)
        {
            m_nextFObj = All.Count - 1;
        }
    }
    /// <summary><paramref name="obj"/>을 등록합니다.</summary>
    /// <param name="obj">등록할 오브젝트입니다.</param>
    public void Alloc(PooledObject obj)
    {
        All.Add(obj);
        if (obj.active == false)
        {
            if (m_nextFObj == -1)
            {
                m_nextFObj = All.Count - 1;
            }
        }
    }

    /// <summary>비활성화된 오브젝트들 중 하나를 가져와 활성화 시킨 후 반환합니다.</summary>
    public PooledObject Get()
    {
        var ret = GetWithoutSetActive();
        SetActive(ret, true);
        return ret;
    }
    /// <summary>비활성화된 오브젝트들 중 하나를 가져와 반환합니다.</summary>
    public PooledObject GetWithoutSetActive()
    {
        if (m_nextFObj == -1)
        {
            Alloc();
        }
        PooledObject ret = All[m_nextFObj];
        ret.active = true;
        int i = m_nextFObj + 1; // 현재의 nextFObj는 이제 active = true가 되었으니 다음것부터
        m_nextFObj = -1;
        int len = All.Count;
        for (i = m_nextFObj + 1; i < len; i++)
        {
            if (!All[i].active)
            {
                m_nextFObj = i;
                break;
            }
        }
        return ret;
    }

    public void Return(T obj)
    {
        PooledObject e = null;
        int len = All.Count;
        int i;
        for (i = 0; i < len; i++)
        {
            e = All[i];
            if (e.@object == obj)
            {
                break;
            }
        }
        if (i != len) // e != null
        {
            SetActive(e, false);
            if (m_nextFObj > i)
            {
                m_nextFObj = i;
            }
        }
    }
    public void ReturnWithoutSetActive(T obj)
    {
        PooledObject e = null;
        int len = All.Count;
        int i;
        for (i = 0; i < len; i++)
        {
            e = All[i];
            if (e.@object == obj)
            {
                break;
            }
        }
        if (i != len) // e != null
        {
            e.active = false;
            if (m_nextFObj > i)
            {
                m_nextFObj = i;
            }
        }
    }
    public void ReturnAll()
    {
        ReturnAllWithoutSetActive();
        int len = All.Count;
        for (int i = 0; i < len; i++)
        {
            SetActive(All[i], false);
        }
    }
    public void ReturnAllWithoutSetActive()
    {
        m_nextFObj = 0;
    }

    private void SetActive(PooledObject obj, bool active)
    {
        if (obj != null)
        {
            obj.active = active;
            m_setActive?.Invoke(obj.@object, active);
        }
    }

    public PooledObject this[int index]
    {
        get
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            int len = All.Count;
            if (index >= len)
            {
#if DEBUG
                Debug.LogWarning("ObjectPool::this[int index] - ([instance field] All).Count is greater than or equal to [parameter] len");
#endif
                if (m_instantiate == null)
                {
#if DEBUG
                    Debug.LogError("ObjectPool::this[int index] - [instance field] m_instantiate is null");
#endif
                    return null;
                }
            }
            for (int i = len; i <= index; i++)
            {
                Alloc();
            }
            return All[index];
        }
    }
}