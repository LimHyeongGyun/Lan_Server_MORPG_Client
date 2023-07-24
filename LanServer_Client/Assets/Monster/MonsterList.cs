#pragma warning disable IDE0032 // Use auto-implemented property
#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private member
using System;
using UnityEngine;

public class MonsterList : MonoBehaviour
{
    private static MonsterList instance;
    public static MonsterList Instance => instance;

    private ObjectPool<ISlimeController> seedPool, harePool, kingPool;
    public ObjectPool<ISlimeController> SeedPool => seedPool;
    public ObjectPool<ISlimeController> HarePool => harePool;
    public ObjectPool<ISlimeController> KingPool => kingPool;

    /// <summary>
    /// <paramref name="monsterIndex"/>과 <paramref name="monsterNum"/>을 이용해 몬스터를 찾아주는 메서드0
    /// </summary>
    /// <param name="monsterNum">0: seed, 1: hare, 2: king</param>
    /// <param name="monsterIndex">index</param>
    public static ISlimeController GetMonster(ushort monsterNum, ushort monsterIndex)
    {
        switch (monsterNum)
        {
            // seed
            case 0:
                return instance.seedPool[monsterIndex].@object;
            // hare
            case 1:
                return instance.harePool[monsterIndex].@object;
            // king
            case 2:
                return instance.kingPool[monsterIndex].@object;
            default:
                throw new ArgumentOutOfRangeException("monsterNum");
        }
    }

    public GameObject prefabSeed, prefabHare;
    public Transform parentSeed, parentHare;
    private ISlimeController InstantiateSeed()
    {
        GameObject gObj = Instantiate(prefabSeed, parentSeed);
        gObj.SetActive(false);
        return gObj.GetComponent<ISlimeController>();
    }
    private ISlimeController InstantiateHare()
    {
        GameObject gObj = Instantiate(prefabHare, parentHare);
        gObj.SetActive(false);
        return gObj.GetComponent<ISlimeController>();
    }
    private void SetActive(ISlimeController obj, bool active)
    {
        obj.gameObject.SetActive(active);
    }

    private void Awake()
    {
        instance = this;
        seedPool = new ObjectPool<ISlimeController>(InstantiateSeed, SetActive);
        harePool = new ObjectPool<ISlimeController>(InstantiateHare, SetActive);
    }
}