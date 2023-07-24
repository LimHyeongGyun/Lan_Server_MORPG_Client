//	<이동>
//	정의:
//		목표지점을 향해 이동하는 기능
//		타겟팅이 된 플레이어가 있을 시:
//			목표지점은 해당플레이어의 위치가 된다.
//			해당 플레이어가 공격범위 안에 들어오면:
//				이동을 멈춤 -> 이어 Attack 상태로 전환
//		타겟팅이 된 플레이어가 없을 시:
//			Idle 상태일 때:
//				목표지점은 몬스터 소환 영역 안에서 무작위 위치가 된다.
//			Return 상태일 때:
//				목표지점은 원래 위치가 된다.
//	평가방법:
//		1.목표지점을 향해 이동하는지
//		2. Idle 상태에서 몬스터 소환 영역 안에서 무작위 위치로 목표지점이 업데이트되는지
//		3. 공격범위 안에 플레이어가 들어오면 이동을 멈추고 Attack 상태로 전환되는지

using UnityEngine;
using UnityEngine.UI;

public sealed class DEBUG_MOVEMENT : ISlimeController
{
	/// <summary>
	/// 1. 목표지점을 향해 이동하는지<br/>
	///	2. Idle 상태에서 몬스터 소환 영역 안에서 무작위 위치로 목표지점이 업데이트되는지<br/>
	///	3. 공격범위 안에 플레이어가 들어오면 이동을 멈추고 Attack 상태로 전환되는지<br/>
	/// </summary>
	[SerializeField] private int testIndex;
	[SerializeField] private Text text_state;

	private new void OnEnable()
	{
		base.OnEnable();
		switch (testIndex)
		{
			case 1:
				Idle_wanderTimer.elapsed = float.NegativeInfinity;
				// ChangeState(StateEnum.Idle);
				break;
			case 2:
				// ChangeState(StateEnum.Idle);
				break;
			case 3:
				Idle_wanderTimer.elapsed = float.NegativeInfinity;
				break;
		}
	}

	protected override void EnterState(StateEnum state)
	{
		switch (state)
		{
			case StateEnum.Idle:
				if (testIndex == 3)
				{
					Idle_wanderTimer.elapsed = float.NegativeInfinity;
				}
				break;
			case StateEnum.Walk:
				base.Walk_OnEnter();
				break;
			case StateEnum.Attack:
				base.Attack_OnEnter();
				break;
		}
		text_state.text = state.ToString();
	}

	protected override void ExitState(StateEnum state)
	{
		switch (testIndex)
		{
			case 1:
				break;
			case 2:
				break;
			case 3:
				base.ExitState(state);
				break;
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		switch (testIndex)
		{
			case 1:
				if (Input.GetMouseButtonDown(0))
				{
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info))
					{
						_agent.destination = info.point;
					}
				}
				break;
			case 2:
				break;
			case 3:
				if (Input.GetKeyDown(KeyCode.A))
				{
					Targeting(FindObjectOfType<Player>());
				}
				break;
		}
	}
}