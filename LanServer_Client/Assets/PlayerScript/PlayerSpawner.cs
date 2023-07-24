using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSpawner : MonoBehaviour
{
    private Stats worriorStats = new Stats(200, 50, 30, 0.8f, 2.2f, 0.02f, 0.01f);
    private Stats mageStats = new Stats(100, 100, 10, 0.7f, 2f, 0.01f, 0.025f);
    private Stats archerStats = new Stats(130, 80, 5, 1f, 2.5f, 0.012f, 0.01f);
    private All all = new All(1, 0, 0);

    public GameObject worrior;
    public GameObject mage;
    public GameObject archer;

    public void Spawn(int type)
    {
        if (type == 0)
        {
            var player = Instantiate(worrior).GetComponentInChildren<Player>();
            player.Stats = worriorStats;
            player.All = all;
        }
        else if (type == 1)
        {
            var player = Instantiate(mage).GetComponentInChildren<Player>();
            player.Stats = mageStats;
            player.All = all;
        }
        else if (type == 2)
        {
            var player = Instantiate(archer).GetComponentInChildren<Player>();
            player.Stats = archerStats;
            player.All = all;
        }
    }
}