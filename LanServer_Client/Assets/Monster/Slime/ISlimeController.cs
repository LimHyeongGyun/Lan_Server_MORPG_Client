// ???: CommandQueue도 생각해볼만한가
// MonoBehaviour.FixedUpdate() -> MonsterSpawner.FixedUpdate()
// MonoBehaviour.Update() -> MonsterSpawner.Update()

#pragma warning disable IDE0032
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using URandom = UnityEngine.Random; // distinct with System.Random

public struct Timer
{
	public float elapsed;
	public float maxTime;

	public bool Update(float deltaTime, out float over)
	{
        bool flag = UpdateWithoutAutoReset(deltaTime, out over);
        if (flag)
        {
            this.elapsed = 0f;// auto reset
        }
        return flag;
	}
    public bool UpdateWithoutAutoReset(float deltaTime, out float over)
    {
        this.elapsed += deltaTime;
        over = this.elapsed - this.maxTime;
        if (over >= 0f)
        {
            return true;
        }
        return false;
    }

    public float Ratio => Mathf.Clamp01(elapsed / maxTime);
}

/// <summary>
/// Monster Unique IDentifier Interface, MonsterList에서의 인덱스(UID)를 가져오고 설정하는 인터페이스이다.
/// </summary>
public interface IMonsterUID
{
    int GetID();
    void SetID(int Id);
}

[RequireComponent(typeof(SlimeFaceManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class ISlimeController : MonoBehaviour, IMonsterUID
{
    #region IMonsterUID Implements
    int m_uid;
    int IMonsterUID.GetID()
    {
        return m_uid;
    }
    void IMonsterUID.SetID(int Id)
    {
        this.m_uid = Id;
    }
    #endregion

    #region State
    /// <summary>몬스터-슬라임 상태</summary>
    public enum StateEnum
	{
        /// <summary>
        /// Idle
        /// </summary>
		Idle,
        /// <summary>
        /// 타겟팅된 플레이어를 향해 쫓아가는 상태
        /// </summary>
		Walk,
        /// <summary>
        /// 쫓기를 일정시간 안에 성공하지 못하여 돌아가는 상태, 체력 회복도 한다.
        /// </summary>
		Return,
        /// <summary>
        /// 공격범위 안에 있는 타겟을 공격하는 상태
        /// </summary>
		Attack,
        /// <summary>
        /// 몬스터 소환하는 패턴(킹)
        /// </summary>
		Spawn,
        /// <summary>
        /// 스킬1 (캐릭터별로 상이)
        /// </summary>
		Skill1,
        /// <summary>
        /// 스킬2 (캐릭터별로 상이)
        /// </summary>
		Skill2,
		Die,
		/// <summary>
		/// reserved, used in test
		/// </summary>
		Res1,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res2,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res3,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res4,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res5,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res6,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res7,
        /// <summary>
        /// reserved, used in test
        /// </summary>
        Res8,
    }

	#region <Idle>
	protected Timer Idle_wanderTimer = new Timer() { maxTime = 7f };

	/// <summary>
	/// <see cref="StateEnum.Idle"/> 상태 진입 시 호출.<br/>
	/// 1. 타이머 랜덤 초기화.<br/>
	/// 2. 《<see cref="MONSTER_SETTINGS.FACE"/>》얼굴을 Idle 얼굴로 변경.
	/// </summary>
	protected void Idle_OnEnter()
	{
		Idle_wanderTimer.elapsed = Idle_wanderTimer.maxTime * URandom.value;
		if (MONSTER_SETTINGS.FACE)
		{
			this._face.BaseTexture = this._face.face.GetTexture(SlimeFace.Type.Idle);
		}
	}

	protected void Targeting(Player target)
	{
		if (target != null && target.ST != StateType.Die)
		{
			if (target != this._target)
			{
				this._target = target;
				this.ChangeState(StateEnum.Walk);
			}
		}
		else
		{
			this._target = null;
			this.ChangeState(StateEnum.Return);
		}
	}

	/// <summary>
	/// 파라미터들은 스포너가 없을 시 사용됩니다.
	/// </summary>
	/// <param name="radius"></param>
	/// <returns></returns>
	protected virtual Vector3 GetRandomPoint()
	{
		Vector3 rand;
        if (spawner != null)
        {
            rand = this.spawner.GetRandomPoint();
        }
		else
		{
			Vector2 temp = rand = URandom.insideUnitCircle;
			rand = new Vector3(temp.x * 10, 0, temp.y * 10);
		}
		return rand;
    }

	/// <summary>
	/// <see cref="StateEnum.Idle"/> 상태 FixedUpdate.<br/>
	/// 1. Target이 발견되면 <see cref="_target"/>을 해당 타겟으로 지정 후 상태를 <see cref="StateEnum.Walk"/>로 변경.
	/// 2. 일정 시간마다 몬스터 소환 영역 안에서 무작위 위치로 이동<br/>
	/// </summary>
	protected void Idle_OnFixedUpdate()
	{
		// 1. 타겟팅
		if (this.GetNextTarget(out Player target))
		{
			Targeting(target);
			return;
		}
		// 2. 무작위 이동
		if (Idle_wanderTimer.Update(Time.fixedDeltaTime, out _))
		{
			Idle_wanderTimer.elapsed = 0f;
			Vector3 rand = GetRandomPoint();
			if (Physics.Raycast(new Vector3(rand.x, 1e+10f, rand.z), Vector3.down, out RaycastHit hit))
			{
				this._agent.destination = hit.point;
			}
		}
    }
	#endregion

	#region <Walk>
	protected Timer Walk_chaseTimer = new Timer() { maxTime = 10f };

	/// <summary>
	/// <see cref="StateEnum.Walk"/> 상태 진입 시 호출.<br/>
	/// 1. 타이머 초기화
	/// 2. 《<see cref="MONSTER_SETTINGS.FACE"/>》얼굴을 Walk 얼굴로 변경.
	/// </summary>
	protected void Walk_OnEnter()
	{
		Walk_chaseTimer.elapsed = 0f;
		if (MONSTER_SETTINGS.FACE)
		{
			this._face.BaseTexture = this._face.face.GetTexture(SlimeFace.Type.Walk);
		}
	}

	/// <summary>
	/// Walk 상태 FixedUpdate().<br/>
	/// 움직일 위치를 계속 타겟의 위치로 업데이트 해 주며, 거리가 공격범위 내로 들어오면 Attack 상태로 전환,<br/>
	/// 일정 시간동안 쫓는데 공격범위 내로 들어오지 않을 경우 <see cref="StateEnum.Return"/> 상태로 전환시킨다.
	/// </summary>
	protected void Walk_OnFixedUpdate()
	{
		this._agent.destination = this._target.transform.position;
		if (_attackRange.EnteredPlayer.Contains(this._target))
		{
            NextAttack();
			//this.ChangeState(StateEnum.Attack);
		}
		else
		{
			if (Walk_chaseTimer.Update(Time.fixedDeltaTime, out _))
			{
				Targeting(null);
			}
		}
	}
	#endregion

	#region <Return>
	private Timer Return_HPGenTimer = new Timer() { maxTime = 1f };

	/// <summary>
	/// <see cref="StateEnum.Return"/> 상태 진입 시 호출.<br/>
	/// 1. 회복 타이머 초기화.<br/>.
	/// 2. 돌아갈 지점을 소환지점 중심으로 지정.<br/>
	/// 3. 《<see cref="MONSTER_SETTINGS.FACE"/>》얼굴을 Walk 얼굴로 변경.
	/// </summary>
	protected void Return_OnEnter()
	{
		Return_HPGenTimer.elapsed = 0f;
		this._agent.destination = this.spawner != null ? this.spawner.transform.position : this.spawnPoint;
		if (MONSTER_SETTINGS.FACE)
		{
			this._face.BaseTexture = this._face.face.GetTexture(SlimeFace.Type.Walk);
		}
	}

	/// <summary>
	/// <see cref="StateEnum.Return"/> 상태의 FixedUpdate().<br/>
	/// 1. 1초 마다 체력 회복<br/>
	/// 2. 목표 지점 도착 및 체력이 최대치에 도달할 경우 <see cref="StateEnum.Idle"/> 상태로 변환
	/// </summary>
	protected void Return_FixedUpdate()
	{
		// 체력 회복
		if (Return_HPGenTimer.Update(Time.fixedDeltaTime, out _))
		{
			Return_HPGenTimer.elapsed = 0f;
			this.IncreaseHP(this.stat.maxHP * 0.1f); // 10%
		}
		// 도착 여부 판단
		if (this.HP >= this.stat.maxHP * 0.9999f &&
			((this.spawner != null && this.spawner == this.spawner.area.EnteredMonster.Contains(this)) ||
			(this.spawner == null && this._agent.remainingDistance < this._agent.stoppingDistance)))
		{
			this.ChangeState(StateEnum.Idle);
		}
	}
	#endregion

	#region <Attack>
	private bool Attack_attackStarted = false;
	private bool Attack_attackEventTrigger = false;
	private bool Attack_attackEndTrigger = false;
    public Collider attackCollider;

	/// <summary>
	/// <see cref="StateEnum.Attack"/> 상태 진입 시 호출.<br/>
	/// 1. 트리거 및 플래그들 초기화.<br/>
	/// 2. 얼굴 변경
	/// </summary>
	protected void Attack_OnEnter()
	{
		Attack_attackStarted = false;
		Attack_attackEventTrigger = false;
		Attack_attackEndTrigger = false;
		if (MONSTER_SETTINGS.FACE)
		{
			this._face.BaseTexture = this._face.face.GetTexture(SlimeFace.Type.Attack);
		}
	}

	/// <summary>
	/// <see cref="StateEnum.Attack"/> 상태의 애니메이션 트리거.<br/>
	/// attackEventTrigger 또는 attackEndTrigger 활성화시킨다.
	/// </summary>
	protected void Attack_OnAnimEvent(AnimEventType type)
	{
		switch (type)
		{
			case AnimEventType.Dammage:
				Attack_attackEventTrigger = true;
				break;
			case AnimEventType.End:
				Attack_attackEndTrigger = true;
				break;
		}
	}

	/// <summary>
	/// 행동이 끝난 후 다음 공격을 지정합니다.
	/// 기본(ISlimeController)은 돌격입니다.
	/// </summary>
	protected virtual void NextAttack()
	{
		Player player;
		GetNextTarget(out player);
		if (player == _target)
		{
            ChangeState(StateEnum.Attack);
		}
		else
		{
			Targeting(player);
		}
	}
	/// <summary>
	/// 돌진 데미지
	/// </summary>
	protected virtual float BaseDmg => 128;
	/// <summary>
	/// <see cref="StateEnum.Attack"/> 상태 FixedUpdate().<br/>
	/// 공격이 시작되었다면,<br/>
	/// attackEventTrigger가 활성화 되면 거리를 측정해 범위 내에 있는 경우 플레이어에게 대미지를 입힌다.<br/>
	/// attackEndTrigger가 활성화되면, 타겟이 범위 밖에 있다면 Walk 상태로 변환시키고, 그렇지 않다면 다시 공격 상태로 되돌린다.<br/>
	/// 공격이 시작되지 않았다면,<br/>
	/// 대상이 범위 내에 있다면 보는 방향을 업데이트하고,<br/>
	/// 범위를 벗어나면 Walk 상태로 전환한다.
	/// </summary>
	protected void Attack_OnFixedUpdate()
	{
		bool entered = _attackRange.EnteredPlayer.Contains(this._target);
		if (Attack_attackStarted)
		{
            attackCollider.enabled = true;
			_agent.isStopped = true;
			if (Attack_attackEventTrigger)
			{
				Attack_attackEventTrigger = false;
				if (entered)
				{
                    //this._target.IncreaseHP(-BaseDmg);
                    attackCollider.enabled = false;
				}
			}
			if (Attack_attackEndTrigger)
			{
				_agent.isStopped = false;
				Attack_attackEndTrigger = false;
				if (entered & _target.ST != StateType.Die)
				{
					NextAttack();
				}
				else
				{
					Targeting(null);
				}
			}
		}
		else
		{
			if (_target.ST == StateType.Die)
			{
				Targeting(null);
			}
			else
			{
				this._agent.destination = this._target.transform.position;
				Vector3 v1 = this.transform.forward;
				Vector3 v2 = (this._target.transform.position - this.transform.position);
				Vector2 v3 = new Vector2(v1.x, v1.z).normalized;
				Vector2 v4 = new Vector2(v2.x, v2.z).normalized;
				float view = Vector2.Dot(v3, v4);
				if (entered)
				{
					if (view > 0.98f)
					{
						//Debug.Log("Attack!");
						Attack_attackStarted = true;
						this._animator.SetTrigger("Attack");
					}
					else
					{
						// in Complex: v4 / v3 = v4 * 공액(v3) = (x4 + y4i)(x3 - y3i)
						// = x4x3 + y4y3 + y4x3i - y3x4i
						v4 = new Vector2(v4.x * v3.x + v4.y * v3.y, v4.y * v3.x - v3.y * v4.x);
						float theta = Mathf.Atan2(v4.y, v4.x);
						this.transform.Rotate(0, Mathf.Sign(theta) * Time.fixedDeltaTime * -120, 0);
					}
				}
				else
				{
					this.ChangeState(StateEnum.Walk);
				}
			}
		}
	}

	/// <summary>
	/// <see cref="StateEnum.Attack"/> 상태 진출 시 호출된다.<br/>
	/// 위치 잠금을 해제한다.
	/// </summary>
	protected void Attack_OnExit()
	{
		this._agent.nextPosition = this.transform.position;
		//this._agent.updatePosition = true;
	}
    #endregion

    #region <Die>
    protected void Die_OnEnter()
    {
        Die_animationEndFlag = false;
        _animator.SetBool("Die", true);
    }

    protected void Die_OnAnimEvent(AnimEventType type)
    {
        if (type == AnimEventType.End && _animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            Die_animationEndFlag = true;
        }
    }

    private bool Die_animationEndFlag;
    protected void Die_OnUpdate()
    {
        if (Die_animationEndFlag)
        {
            _animator.SetBool("Die", false);
            Return();
        }
    }
    #endregion

    private StateEnum _state;
	public StateEnum State => _state;
	#endregion

	#region Region(Spawner) & TriggerArea
	/// <summary>
	/// 자신을 소환한 스포너
	/// </summary>
	[HideInInspector] public MonsterSpawner spawner;
	/// <summary>
	/// 인식 범위
	/// </summary>
	[SerializeField] protected TriggerArea _perceiveArea;
	/// <summary>
	/// 공격 범위
	/// </summary>
	[SerializeField] protected TriggerArea _attackRange;
	#endregion
	#region Stat
	[Header("Stat")]
	[SerializeField] protected MonsterStat stat;
	[SerializeField] protected Guage hpGuage;
	public float HP { get; private set; }
	public bool IsDead { get; private set; }

    [SerializeField] private GameObject dmgTextPrefab;

	/// <summary>
	/// 슬라임의 스탯에 관하여 초기화한다.
	/// </summary>
	private void Init_Stat()
	{
		this.HP = stat.maxHP;
		this.IsDead = false;
		this._agent.speed = stat.moveSpeed;
		this._perceiveArea.GetComponent<SphereCollider>().radius = this.stat.perceiveRange / 2.0375f; // Inv Scale
		this._perceiveArea.GetComponent<SphereCollider>().radius = this.stat.attackRange / 2.0375f; // Inv Scale
		this._animator.SetFloat("Attack Speed", stat.attackSpeed);
		hpGuage.SetValue(1f);
	}

	/// <summary>
	/// 체력 변화량이 음수일 때의 특수 행동.<br/>
	/// 1. HP가 0 이하로 내려가면 OnDie()가 호출된다.<br/>
	/// 2. 대미지 텍스트가 출력이 된다.
	/// </summary>
	private void _DecreaseHP(float delta)
	{
		if (HP <= 0f)
		{
			HP = 0f;
			OnDie();
		}
		if (MONSTER_SETTINGS.DMG)
		{
            if (dmgTextPrefab == null)
            {
                Debug.LogError("ISlimeController::dmgTextPrefab is null");
            }
            else
            {
                GameObject dmgText = Instantiate(dmgTextPrefab);
                dmgText.transform.position = this.transform.position;
                dmgText.GetComponent<Text>().text = (-delta).ToString();
            }
		}
		if (MONSTER_SETTINGS.FACE)
		{
			_face.duration = 0.75f;
			_face.OverrideTexture = _face.face.GetTexture(SlimeFace.Type.Damage);
		}
	}
	/// <summary>
	/// 체력 변화량이 양수일 때의 특수 행동.<br/>
	/// 1. 체력이 최대채력을 넘지 않도록 제한한다.
	/// </summary>
	private void _IncreaseHP(float delta)
	{
		if (HP > stat.maxHP)
		{
			HP = stat.maxHP;
		}
	}
	/// <summary>
	/// amount만큼 체력을 증가시킨다. amount가 음수일 경우 체력은 감소한다.
	/// </summary>
	public void IncreaseHP(float amount)
	{
		float temp = HP;
		HP = HP + amount;
		float delta = HP - temp;
		if (amount != 0f)
		{
			if (delta > 0f)
			{
				_IncreaseHP(delta);
			}
			else
			{
                OnHit();
				_DecreaseHP(delta);
			}
			hpGuage.SetValue(HP / stat.maxHP);
		}
	}
    private void OnHit()
    {
        _face.OverrideColor = Color.red;
    }

	/// <summary>
	/// 몬스터가 죽을 때 호출이 된다. 상태는 Die가 된다.
	/// </summary>
	private void OnDie()
	{
		IsDead = true;
		Debug.Log("Slime is dead...");
		ChangeState(StateEnum.Die);
	}

	private void Return()
	{
		if (this.spawner != null)
		{
			this.spawner.OnSlimeDead(this);
		}
		else
		{
			Destroy(base.gameObject);
		}
	}
	#endregion

	protected Player _target;
	protected NavMeshAgent _agent;

	#region Animation
	protected Animator _animator;
	protected SlimeFaceManager _face;
    #endregion

    /// <summary>
    /// MonoBehaviour.Awake<br/>
    /// 기본 초기화
    /// </summary>
    protected void Awake()
	{
		_face = GetComponent<SlimeFaceManager>();
		_animator = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		hpGuage.Init();
	}

	public Vector3 spawnPoint;
	/// <summary>
	/// MonoBehaviour.OnEnable<br/>
	/// 스폰 될 시 자동으로 호출되는 초기화 메서드<br/>
	/// 1. 상태를 <see cref="StateEnum.Idle"/>로 초기화
	/// 2. 스탯 초기화
	/// </summary>
	protected void OnEnable()
	{
		this._state = StateEnum.Idle;
		Init_Stat();
		spawnPoint = transform.position;
	}

	/// <summary>
	/// 애니메이션 이벤트 유형
	/// </summary>
	public enum AnimEventType
	{
		Start = 0,
		End = 1,
		Dammage = 2
	}
	/// <summary>
	/// 애니메이션 이벤트
	/// </summary>
	/// <param name="type"></param>
	public virtual void AnimEvent(int type)
	{
		switch (this._state)
		{
			case StateEnum.Attack:
				Attack_OnAnimEvent((AnimEventType)type);
				break;
		}
	}
	/// <summary>
	/// MonoBehaviour.Update()<br/>
	/// Set Walk Animation Speed
	/// </summary>
	public virtual void OnUpdate()
	{

	}
	/// <summary>
	/// MonoBehaviour.FixedUpdate()
	/// </summary>
	public virtual void OnFixedUpdate()
	{
		switch (this._state)
		{
			case StateEnum.Idle:
				Idle_OnFixedUpdate();
				break;
			case StateEnum.Walk:
				Walk_OnFixedUpdate();
				break;
			case StateEnum.Return:
				Return_FixedUpdate();
				break;
			case StateEnum.Attack:
				Attack_OnFixedUpdate();
				break;
		}
		if (this.State != StateEnum.Attack)
		{
			_animator.SetFloat("Speed", (Mathf.Clamp01(_agent.remainingDistance / (_agent.stoppingDistance * 2)) - 1f) * 1.5f + 1f);
            _agent.isStopped = false;
        }
        else
		{
			_animator.SetFloat("Speed", 0f);
			_agent.isStopped = true;
		}
		//_animator.SetFloat("Speed", _agent.remainingDistance < _agent.stoppingDistance ? 0f : 1f);
	}

	/// <summary>
	/// 상태 전환 시 호출, 슬라임이 <paramref name="state"/>에서 진출할 때 호출될 함수를 호출하는 메서드
	/// </summary>
	protected virtual void ExitState(StateEnum state)
	{
		switch (state)
		{
			case StateEnum.Attack:
				Attack_OnExit();
				break;
		}
	}
	/// <summary>
	/// 상태 전환 시 호출, 슬라임이 <paramref name="state"/>로 진입할 때 호출할 함수를 호출하는 메서드
	/// </summary>
	protected virtual void EnterState(StateEnum state)
	{
		switch (this._state)
		{
			case StateEnum.Idle:
				Idle_OnEnter();
				break;
			case StateEnum.Walk:
				Walk_OnEnter();
				break;
			case StateEnum.Return:
				Return_OnEnter();
				break;
			case StateEnum.Attack:
				Attack_OnEnter();
				break;
		}
	}
    public bool lockState = false;
	/// <summary>
	/// 슬라임의 <see cref="_state"/>를 <paramref name="state"/>로 상태를 전환한다.
	/// </summary>
	public void ChangeState(StateEnum state, bool forcely = false)
	{
        if (forcely == true || !lockState)
        {
            //Debug.Log(string.Format("{0}->{1}", this.State, state));
            ExitState(this._state);
            EnterState(this._state = state);
        }
	}

	/// <summary>
	/// 다음 타겟을 찾는다.<br/>
	/// 타겟이 발견되면 반환값이 true며, <paramref name="target"/>에 해당 플레이어(타겟)가 담긴다.<br/>
	/// 그렇지 않으면 반환값이 false며, <paramref name="target"/>에 null이 담긴다.
	/// </summary>
	public bool GetNextTarget(out Player target)
	{
		float minDistance = float.PositiveInfinity;
		target = spawner?.area.GetNearestPlayer(transform.position, out minDistance);
		if (target == null || minDistance > stat.perceiveRange)
		{
			target = _perceiveArea.GetNearestPlayer(transform.position, out minDistance);
		}
		return target != null;
	}

    private void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = _animator.rootPosition;
        position.y = _agent.nextPosition.y;
        _agent.nextPosition = transform.position = position;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}