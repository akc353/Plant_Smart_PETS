using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponenetExtensions
{
    public static void Reset(this AnimationState AnimState)
    {
        AnimState.time = 0;
        AnimState.speed = 0;
    }

    public static void SetTime(this AnimationState AnimState, float time)
    {
        AnimState.time = time;
        AnimState.speed = 0;
    }

    public static List<Transform> GetChilidList(this Transform parentTrans)
    {
        int Cnt = parentTrans.childCount;
        List<Transform> childList = new List<Transform>();

        for(int i = 0; i < Cnt; i++)
        {
            childList.Add(parentTrans.GetChild(i));
        }

        return childList;
    }
}