#pragma warning disable IDE0051

using System;
using UnityEngine;
using UnityEngine.UI;

public class DEBUG_TRIGGER : MonoBehaviour
{
    public TriggerArea area;
    public Text text;

    private void Update()
    {
        string str = null;
        foreach (var a in area.EnteredPlayer)
        {
            str = Environment.NewLine + a.name + str;
        }
        str = Environment.NewLine + "<PlayerList>" + str;
        foreach (var a in area.EnteredMonster)
        {
            str = Environment.NewLine + a.name + str;
        }
        str = "<MonsterList>" + str;
        text.text = str;
    }
}
