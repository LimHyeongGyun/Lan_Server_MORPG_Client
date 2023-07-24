using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGuage : MonoBehaviour
{
    public Slider hpbar;
    public Player player;
    void Start()
    {
        hpbar = GetComponentInChildren<Slider>();
    }

    void Update()
    {
        transform.position = player.transform.position;
        hpbar.value = player.curHP / player.stats.maxHp;
    }
}
