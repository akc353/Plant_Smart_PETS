using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

class MyButton : MyControl
{
    public System.Action action = null;
    public override void Init(string _name, ControlType _type)
    {
        base.Init(_name, ControlType.Button);

        if (ReadFile.Inst.buttonPath.ContainsKey(gameObject.name.Split('_')[0]))
        {
            action = () =>
            {
                Debug.Log(ReadFile.Inst.buttonPath[gameObject.name.Split('_')[0]]);
                System.Diagnostics.Process.Start(ReadFile.Inst.buttonPath[gameObject.name.Split('_')[0]]);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            };
        }
    }

    
    //버튼선택했을 때 동작할 함수
    public void Run()
    {
        if (action != null)
            action.Invoke();

        Debug.Log("Button(Run) :" + gameObject.name.Split('_')[0]);
    }
}