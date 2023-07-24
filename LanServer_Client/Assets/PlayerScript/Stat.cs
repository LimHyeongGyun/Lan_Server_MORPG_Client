using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stats
{
    public float maxHp;
    public float maxMp;
    public float damage;
    public float attackSpeed;
    public float moveSpeed;
    public float genHp;
    public float genMp;

    public Stats(float MaxHp, float MaxMp, float Damage, float AttackSpeed, float MoveSpeed ,float PlusHp, float PlusMp)
    {
        maxHp = MaxHp;
        maxMp = MaxMp;
        damage = Damage;
        attackSpeed = AttackSpeed;
        moveSpeed = MoveSpeed;
        genHp = PlusHp;
        genMp = PlusMp;
    }
}
public struct All
{
    public int level;
    public int gold;
    public int exp;
    public All(int Level, int Gold, int Exp)
    {
        level = Level;
        exp = Exp;
        gold = Gold;
    }
}
public enum JobType { Worrior, Mage, Archer }
public enum StateType { Playing, Die, Unbeatable }
public enum AnimType { Idle, Walk, Attack, Skill1, Skill2, Skill3}