using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGM : GameMode {

    public override IEnumerator StartGame()
    {
        PlayerMgr.Inst.Get("RightHand").SetActive(false);
        PlayerMgr.Inst.Get("LeftHand").SetActive(false);

        if (Get("TabBox_Small") != null)
            Get("TabBox_Small").SetActive(false);
        Get("TabBox_Obj").SetActive(false);
        yield break;
    }
}
