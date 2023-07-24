#pragma warning disable IDE0051 // Remove unused private member
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerArea : MonoBehaviour
{
	public HashSet<Player> EnteredPlayer { get; private set; } = new HashSet<Player>();
	public HashSet<ISlimeController> EnteredMonster { get; private set; } = new HashSet<ISlimeController>();

	/// <summary>
	/// Unity MonoBehaviour.OnDisable()<br/>
	/// <see cref="EnteredPlayer"/>와 <see cref="EnteredMonster"/> 리스트를 초기화(Clear)합니다.
	/// </summary>
	private void OnDisable()
	{
		EnteredPlayer.Clear();
		EnteredMonster.Clear();
	}

	/// <summary>
	/// Unity MonoBehaviour.OnTriggerEnter()<br/>
	/// <paramref name="other"/>가 플레이어일 시 <see cref="EnteredPlayer"/>에 추가됩니다.<br/>
	/// <paramref name="other"/>가 몬스터(슬라임)일 시 <see cref="EnteredMonster"/>에 추가됩니다.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
        if (other.TryGetComponent(out ISlimeController monster))
		{
			EnteredMonster.Add(monster);
		}
		else
		{
			if (other.TryGetComponent(out Player player))
			{
				EnteredPlayer.Add(player);
			}
		}
	}

	/// <summary>
	/// 추가 이유: 단순 트리거 비활성화로는 OnTriggerExit가 호출되지는 않는다. OnTriggerEnterer는 호출된다.
	/// </summary>
    private void FixedUpdate()
    {
		EnteredMonster.RemoveWhere(monster => !monster.isActiveAndEnabled);
		EnteredPlayer.RemoveWhere(player => !player.isActiveAndEnabled);
    }

    /// <summary>
    /// Unity MonoBehaviour.OnTriggerEnter()<br/>
    /// <paramref name="other"/>가 플레이어일 시 <see cref="EnteredPlayer"/>에서 제거됩니다.<br/>
    /// <paramref name="other"/>가 몬스터(슬라임)일 시 <see cref="EnteredMonster"/>에서 제거됩니다.
    /// </summary>
    private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out ISlimeController monster))
		{
			EnteredMonster.Remove(monster);
		}
		else
		{
			if (other.TryGetComponent(out Player player))
			{
				EnteredPlayer.Remove(player);
			}
		}
	}

	/// <summary>
	/// 현재 트리거 안에 들어온 플레이어 중에서 <paramref name="position"/>으로 부터 가장 가까운 지점에 있는 플레이어를 반환합니다.<br/>
	/// 플레이어가 발견된 경우 <paramref name="minDistance"/>에 거리의 제곱이 담기며, 그렇지 않은 경우 <see cref="float.PositiveInfinity"/>가 담깁니다.<br/>
	/// 거리는 높이를 무시한 XZ 평면에서의 값입니다.
	/// </summary>
	public Player GetNearestPlayer(Vector3 position, out float minDistanceSqr)
	{
		minDistanceSqr = float.PositiveInfinity;
		Player ret = null;
		foreach (Player player in EnteredPlayer)
		{
			if (player == null || player.ST == StateType.Die || !player.gameObject.activeInHierarchy)
			{
				continue;
			}
			float distanceSqr = DistanceXZSqr(player.transform.position, position);
			if (distanceSqr < minDistanceSqr)
			{
				minDistanceSqr = distanceSqr;
				ret = player;
			}
		}
		return ret;
	}

	/// <summary>
	/// XZ 평면에서의 거리의 제곱를 계산합니다.
	/// </summary>
    private static float DistanceXZSqr(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return dx * dx + dz * dz;
    }
}