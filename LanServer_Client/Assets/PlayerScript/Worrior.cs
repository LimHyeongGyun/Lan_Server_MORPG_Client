using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worrior : Player
{
    bool attack;
    Animation anima;
    WaitForSeconds WFS06 = new WaitForSeconds(0.6f);
    protected override void Start()
    {
        base.Start();
        weaponpos = gameObject.GetComponentInChildren<BoxCollider>(); //������ ���� �ݶ��̴��� �޾ƿ�
        weaponpos.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (ST != StateType.Die && (AT == AnimType.Idle || AT == AnimType.Walk))
            AttackInput();
    }

    void AttackInput()
    {
        if (!attack && Input.GetMouseButton(0))
            StartCoroutine(AttackAction());
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(Smite());
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(Provocation());
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(BearBounce());
    }
    /// <summary>
    /// �⺻����
    /// </summary>
    IEnumerator AttackAction()
    {
        attack = true;
        AT = AnimType.Attack;

        anim.SetTrigger("Attack");
        anim.SetFloat("AttackSpeed", stats.attackSpeed);
        yield return WFS06;

        AT = AnimType.Idle;
        delay = 0;
        yield return new WaitUntil(() => delay > 1/stats.attackSpeed);

        attack = false;
    }
    public void On() { weaponpos.enabled = true; }
    public void Off() { weaponpos.enabled = false; }

    /// <summary>
    /// ��ų
    /// </summary>
    /// <returns></returns>
    IEnumerator Smite()//��Ÿ
    {
        AT = AnimType.Skill1;
        Debug.Log("Smite");
        anim.SetTrigger("Smite");
        yield return null;

        AT = AnimType.Idle;
    }
    IEnumerator Provocation() //����
    {
        AT = AnimType.Skill2;
        Debug.Log("Provocation");
        anim.SetTrigger("Provocation");
        yield return null;

        AT = AnimType.Idle;
    }
    IEnumerator BearBounce()//����Ȱ��
    {
        AT = AnimType.Skill3;
        Debug.Log("BearBounce");
        anim.SetTrigger("BearBounce");
        yield return null;

        AT = AnimType.Idle;
    }
}