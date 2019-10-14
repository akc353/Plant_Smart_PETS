using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicUpdate
{
    public string name;
    public System.Func<bool> Condition;
    public System.Action CallBack;
    public bool autoDisconnect;
    public bool isConnect;

    public DynamicUpdate(string name, bool autoDisconnect, Func<bool> Condition, Action Callback)
    {
        this.Condition = Condition;
        this.CallBack = Callback;
        this.autoDisconnect = autoDisconnect;

        Connect();
    }

    public void UpdateGuest()
    {
        if (Condition.Invoke())
        {
            CallBack.Invoke();

            if (autoDisconnect)
                DisConnect();
        }
    }

    public void Connect()
    {
        isConnect = true;
        GameState.Inst.SubscribePlayUpdate(UpdateGuest);
 
    }

    public void DisConnect()
    {
        isConnect = false;
        GameState.Inst.UnSubscribePlayUpdate(UpdateGuest);
       
    }
}
