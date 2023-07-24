using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archer : Player
{
    public GameObject arrow;
    public Image img_WindBlessing;
    public Image img_LonginusSpear;

    int count;
    [HideInInspector]public float arrowdmg;
    bool attack;
    bool skill1;
    bool skill3;

    Transform arrowpos;
    Weapon WPdmg;
    WaitForSeconds WFS035 = new WaitForSeconds(0.35f);
    WaitForSeconds WFS5 = new WaitForSeconds(5f);
    WaitForSeconds WFS2 = new WaitForSeconds(2f);

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        weaponpos = gameObject.GetComponentInChildren<BoxCollider>(); //ȭ���� �߻�Ǵ� ��ġ
        arrowpos = weaponpos.GetComponentInChildren<BoxCollider>().transform; //ȭ�� ���� �� rigidbody.velocity����� ���� ȭ����ǥ
    }

    protected override void Update()
    {
        base.Update();
        if(ST != StateType.Die && (AT == AnimType.Idle || AT == AnimType.Walk))
            AttackInput();
    }
    void AttackInput()
    {
        if (!attack && Input.GetMouseButton(0))
            AttackAnim(AnimType.Attack);
        else if (Input.GetKeyDown(KeyCode.Alpha1) && curMP > 30 && !skill1)
            AttackAnim(AnimType.Skill1);
        else if (all.level > 2 && Input.GetKeyDown(KeyCode.Alpha3) && curMP > 60 && !skill3)
            AttackAnim(AnimType.Skill3);
    }
    void AttackAnim(AnimType type)
    {
        switch (type)
        {
            case AnimType.Attack:
                StartCoroutine(AttackAction());
                break;
            case AnimType.Skill1:
                {
                    StartCoroutine(WindBlessing());
                    StartCoroutine(CoolTime(img_WindBlessing, 20f, skill1));
                }
                break;
            case AnimType.Skill3:
                {
                    StartCoroutine(LonginusSpear());
                    StartCoroutine(CoolTime(img_LonginusSpear, 25f, skill3));
                }
                break;
        }
    }

    /// <summary>
    /// �⺻����
    /// </summary>
    IEnumerator AttackAction() //���� �ִϸ��̼�
    {
        attack = true;
        anim.SetTrigger("Attack");
        anim.SetFloat("AttackSpeed", stats.attackSpeed);
        yield return WFS035;

        if (all.level == 1)
            arrowdmg = stats.damage;
        else
            SplitArrow();
        Shot();
        AT = AnimType.Idle;
        delay = 0;
        yield return new WaitUntil(() => delay > 1 / stats.attackSpeed);

        attack = false;
    }
    void Shot() //ȭ�����
    {
        GameObject weapon = Instantiate(arrow, arrowpos.position, arrowpos.rotation);
        WPdmg = weapon.GetComponentInChildren<Weapon>();
        WPdmg.playerDMG = arrowdmg;
        Rigidbody weaponr = weapon.GetComponent<Rigidbody>();
        weaponr.velocity = Vector3.Lerp(arrowpos.position, arrowpos.forward*15, 1f);
    }

    /// <summary>
    /// ��ų
    /// </summary>
    /// <returns></returns>
    IEnumerator WindBlessing()//�ٶ��� �ູ
    {
        skill1 = true;
        Debug.Log("�ٶ��� �ູ On");
        anim.SetTrigger("WindBlessing");
        AT = AnimType.Idle;
        curMP -= 30;
        stats.attackSpeed = stats.attackSpeed * 2; //���ݼӵ� 100�ۼ�Ʈ ����
        stats.moveSpeed = stats.moveSpeed * 2; //�̵��ӵ� 100�ۼ�Ʈ ����
        yield return WFS5;

        stats.attackSpeed = stats.attackSpeed / 2; //���ݼӵ� �������
        stats.moveSpeed = stats.moveSpeed / 2; //�̵��ӵ� �������
        Debug.Log("�ٶ��� �ູ Off");
        skill1 = false;
    }
    void SplitArrow()//���ø��ַο�
    {
        count += 1;
        if (count == 3)
        {
            arrowdmg = stats.damage * 1.3f;
            count = 0;
        }
        else
            arrowdmg = stats.damage;

    }
    IEnumerator LonginusSpear() //�ձ⴩���� â
    {
        skill3 = true;
        Debug.Log("LonginusSpear");
        anim.SetTrigger("LonginusSpear");
        yield return WFS2;

        arrowdmg = stats.damage * 10; //10���� ������
        Shot();
        AT = AnimType.Idle;
        curMP -= 60;
        skill3 = false;
    }
    /// <summary>
    /// ��ų ��Ÿ��
    /// </summary>
    /// <param name="image"></param>
    /// <param name="cool"></param>
    /// <returns></returns>
    IEnumerator CoolTime(Image image, float cool, bool skill)
    {
        while (cool > 1.0f)
        {
            cool -= Time.deltaTime;
            image.fillAmount = (1.0f / cool);
            yield return new WaitForFixedUpdate();
        }
        skill = false;
    }
}
