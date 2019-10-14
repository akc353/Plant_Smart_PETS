using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : CollectionableMonoBehaviour
{
    public void Awake()
    {
        if (GameState.Inst != null)
        {
            GameState.Inst.NowLocation = this;
            print("nowLocation");
        }
        //TestFollow에 ....흠..아돈노..왓유원트....
    }
}
