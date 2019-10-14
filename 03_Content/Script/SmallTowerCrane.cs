using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTowerCrane : CollectionableMonoBehaviour {
    [Range(0, 2f)]
    public float Time = 1.67f;
    [Range(0,200f)]
    public float craneIntervalLength = 75.823512f;

    private void Start()
    {
        Get("Hookblock_ani").GetComponent<Animation>().enabled = true;
    }

    private void Update()
    {
        var Pos = Get("Trolley").transform.localPosition;
        Pos.x = Get("[P]Goal2").transform.localPosition.x;
        Get("Trolley").transform.localPosition = Pos;

        MoveHook();
    }

    public void MoveHook()
    {
        var goal = Get("[P]Goal2");
        var hookani = Get("Hookblock_ani");
        var height = goal.transform.position.y;

        var craneHeight = this.gameObject.transform.position.y;
        var craneHookAnimMin = 3.2f + craneHeight;
        var craneHookAnimMax = 82.18607f + craneHeight;

        if (height < craneHookAnimMin || craneHookAnimMax < height)
        {
            return;
        }
        else
        {
            var lerpHeight = (height - craneHookAnimMin) / craneIntervalLength;
            var time = Mathf.Lerp(0, Time, (float)lerpHeight);
            hookani.Get<Animation>()["Take 001"].Reset();
            hookani.Get<Animation>().Play();
            hookani.Get<Animation>()["Take 001"].SetTime(Time - time);
            hookani.Get<Animation>().Play();
        }
    }
}
