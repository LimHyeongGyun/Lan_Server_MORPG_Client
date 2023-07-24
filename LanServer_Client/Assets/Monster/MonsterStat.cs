using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MonsterStat")]
public class MonsterStat : ScriptableObject
{
    /// <summary> 최대 체력 </summary>
    public float maxHP;

    /// <summary> 공격력 </summary>
    public float attack;

    /// <summary> 공격 속도 </summary>
    public float attackSpeed;

    /// <summary> 이동 속도 </summary>
    public float moveSpeed;

    /// <summary> 공격 범위 </summary>
    public float attackRange;

    /// <summary> 인식 범위 </summary>
    public float perceiveRange;

    /// <summary> 경험치 </summary>
    public int exp;

    /// <summary> 보상(gold) </summary>
    public int reward;
}