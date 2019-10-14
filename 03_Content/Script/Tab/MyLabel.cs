using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MyLabel : MyControl
{
    public override void Init(string _name, ControlType _type)
    {
        base.Init(_name, ControlType.Button);
    }
}
