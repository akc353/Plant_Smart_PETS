using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyControl : WindowCtrl
{
    protected string mName;
    protected ControlType mType;

    public ControlType newObjectType;

    protected bool isInit = false;

    public virtual void Init(string _name, ControlType _type) {
        mName = _name;
        mType = _type;
    }

    public string GetName()
    {
        return mName;
    }

    public void SetName ()
    {
        mName = gameObject.name;
    }

    public ControlType GetControlType()
    {
        return mType;
    }
}