using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle,Walk,Jump,Attack,Damage}

public class EnemyAi : MonoBehaviour
{
	public SlimeAnimationState currentState;

	public Animator animator;
	public NavMeshAgent agent;
	public int damType;

	void Update()
	{
		// set Speed parameter synchronized with agent root motion moverment
		animator.SetFloat("Speed", agent.velocity.magnitude);
	}
}