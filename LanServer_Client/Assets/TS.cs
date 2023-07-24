using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TS : MonoBehaviour
{
    public int damage = 10;

    private Player player;
    public GameObject play;

    private void Update()
    {
        if (play == null)
        {
            play = GameObject.FindWithTag("Player"); //작용할 테스트 플레이어 오브젝트
            player = play.GetComponentInChildren<Player>();
        }
    }

    public void PlusHp()
    {
        player.all.level += 1;
        Debug.Log(player.all.level);
    }
    public void MinusHp()
    {
        player.curHP -= 1;
        Debug.Log(player.All.level);
    }
    public void Recovery()
    {
        player.curMP = player.stats.maxMp;
    }
}
