using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public StateType ST;
    public AnimType AT; 
    public static JobType JT;

    public Stats stats = new Stats();
    public All all = new All();
    public Stats Stats
    {
        get { return stats; }
        set { stats = value; }
    }
    public All All
    {
        get { return  all; }
        set { all = value; }
    }

    protected float _timer;
    protected float rate;
    protected float delay;
    public float curHP;
    public float curMP;
    public int curexp;

    public BoxCollider spawnPos;

    protected BoxCollider weaponpos;

    MeshRenderer[] meshs;
    protected Animator anim;
    Rigidbody rigid;

    WaitForSeconds WFS01 = new WaitForSeconds(0.1f);
    WaitForSeconds WFS3 = new WaitForSeconds(3f);
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }
    protected virtual void Start()
    {
        anim.SetBool("Live",true);
        curHP = stats.maxHp;
        curMP = stats.maxMp;
        //spawnpos = spawnPos.GetComponent<MeshCollider>();
    }

    protected virtual void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 1f) //1�ʴ� ȸ��
        {
            Recovery();
            _timer = 0;
        }
        delay += Time.deltaTime; //�⺻���� ���ݼӵ� ������
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MonsterAttack"))
        {
            if (ST == StateType.Playing) //Playing������ ���� ����� �޵���
            {
                TS ma = other.GetComponent<TS>(); //�׽�Ʈ�� ���߿� �ٲ� ��
                curHP -= ma.damage;
                Debug.Log(curHP);
                StartCoroutine(DamageAction());
            }
        }
    }

    void Recovery()
    {
        StartCoroutine(RegainHp());
        StartCoroutine(RegainMp());
    }
    
    /// <summary>
    /// ü��, ���� ȸ��
    /// </summary>
    /// <returns></returns>
    IEnumerator RegainHp()
    {
        if (curHP < stats.maxHp && ST != StateType.Die)
            curHP += stats.maxHp * stats.genHp;
        else if(curHP > stats.maxHp)
            curHP = stats.maxHp;
        yield return null;
    }
    IEnumerator RegainMp()
    {
        if (curMP < stats.maxMp && ST != StateType.Die)
            curMP += stats.maxMp * stats.genMp;
        else if(curMP > stats.maxMp)
            curMP = stats.maxMp;
        yield return null;
    }

    /// <summary>
    /// ������ �Ծ��� �� FX ��� �� ���º�ȭ
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageAction()
    {
        ST = StateType.Unbeatable; //�ǰݽ� ����

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return WFS01; //0.1��

        if (curHP > 0)
        {
            ST = StateType.Playing;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = new Color(1, 1, 1);
        }
        else
        {
            ST = StateType.Die;
            anim.SetBool("Live", false);
            anim.SetTrigger("Die"); //����ִϸ��̼�
            gameObject.layer = 6;//����� ���̾� ����
            yield return WFS3;

            //3�� �� ��Ȱ
            this.transform.position = SpawnSpot(); //������ �̵�
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = new Color(1, 1, 1);
            gameObject.layer = 7;
            ST = StateType.Playing;
            anim.SetBool("Live", true);
            curHP = stats.maxHp;
            curMP = stats.maxMp;
        }
    }

    /// <summary>
    /// ������ġ
    /// </summary>
    /// <returns></returns>
    private Vector3 SpawnSpot()
    {

        Vector3 ss = new Vector3();
        return ss;
    }
}