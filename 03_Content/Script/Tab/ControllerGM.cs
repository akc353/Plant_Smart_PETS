using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGM : GameMode {
    void InitTabBox()
    {
        PlayerMgr.Inst.Get("RightHand").SetActive(true);
        PlayerMgr.Inst.Get("LeftHand").SetActive(true);

        Get("TabBox_Obj").SetActive(true);
        var pos = PlayerMgr.Inst.Get("VRCamera").transform.position;
        pos.z += 3.5f;

        Get("TabBox_Obj").transform.position = pos;

        pos.y += 1f;

        Get("RunIcon").transform.SetParent(Get("Form").transform);
        Get("RunIcon").transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Get("RunIcon").SetActive(true);
        Get("RunIcon").transform.position = pos;
    }

    public override IEnumerator StartGame()
    {
        //TabBox_Obj 위치 초기화
        InitTabBox();

        yield break;
    }
}
