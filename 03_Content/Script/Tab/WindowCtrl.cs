using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCtrl : MonoBehaviour
{
    //WindowCtrl이 자식으로 가진 컨트롤 리스트
    private List<MyControl> Controls;

    private void Awake()
    {
        Controls = new List<MyControl>();
    }
    
    //게임오브젝트의 이름을 해당 컨트롤의 이름으로 동기화
    public void SetName()
    {
        for(int i = 0; i < Controls.Count; i++)
        {
            Controls[i].SetName();
        }
    }

    //컨트롤 추가
    public virtual bool AddControl(MyControl control)
    {
        if (getControl(control.GetName()) != null)
            return false;

        Controls.Add(control);

        return true;
    }


    public void DeleteControl(MyControl control)
    {
        for (int i = 0; i < Controls.Count; i++)
        {
            if (Controls[i].GetName().Equals(control.GetName()))
            {
                Controls.RemoveAt(i);
                break;
            }
        }
    }

    //해당 이름을 가진 컨트롤 반환
    public MyControl getControl(string name)
    {
        for (int i = 0; i < Controls.Count; i++)
        {
            if (Controls[i].GetName().Equals(name))
            {
                return Controls[i];
            }
        }

        return null;
    }

    //해당 인덱스의 컨트롤 반환
    public MyControl getControl(int idx)
    {
        if (idx >= 0 && idx < Controls.Count)
            return Controls[idx];
        else
            return null;
    }
}
