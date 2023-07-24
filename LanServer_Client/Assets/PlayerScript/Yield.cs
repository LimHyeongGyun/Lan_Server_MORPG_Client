using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Yield
{
    //�ڷ�ƾ ����ȭ Ŭ����
    static readonly Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!waitForSeconds.TryGetValue(seconds, out wfs))
        {
            waitForSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));
        }
        return wfs;
    }
}
