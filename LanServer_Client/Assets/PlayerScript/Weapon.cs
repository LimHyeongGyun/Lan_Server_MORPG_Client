using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private const int MonsterLayer = 8;
    private HashSet<ISlimeController> enteredMonster = new HashSet<ISlimeController>();
    [HideInInspector]public float playerDMG;
    void Start()
    {
        if (this.gameObject.CompareTag("Melee"))
            playerDMG = GetComponentInParent<Worrior>().stats.damage;
        Debug.Log(playerDMG);
    }

    void Update()
    {
        Destroyobj();
    }
    public void Init()
    {
        enteredMonster.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MonsterLayer)
        {
            if (this.gameObject.CompareTag("Arrow") || this.gameObject.CompareTag("Magic")) //���Ÿ� ���ݽ�
                StartCoroutine(Shotobj());
            var monster = other.GetComponentInChildren<ISlimeController>();
            if (!enteredMonster.Contains(monster))
            {
                enteredMonster.Add(monster); // [return]bool ���� �̿��� contains ������ �� �� ���� �� �ִ�.
                monster.IncreaseHP(-playerDMG);
            }
        }
    }
    IEnumerator Shotobj()
    {
        Destroy(gameObject);
        yield return null;
        //Particle subEmitter
    }
    void Destroyobj()
    {
        if (this.gameObject.CompareTag("Arrow") || this.gameObject.CompareTag("Magic")) //���Ÿ� ���ݽ�
            Destroy(gameObject, 4);
    }
}
