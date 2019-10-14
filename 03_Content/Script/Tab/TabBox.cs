using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TabBox : MyControl
{
    private List<MyTab> TabList;
    public int SelectedIndex = 0;
    
    public override void Init(string _name, ControlType _type)
    {
        base.Init(_name, ControlType.TabBox);
        if (!isInit)
        {
            TabList = new List<MyTab>();

            AddAllObject(gameObject, TabList);
            isInit = true;
        }

        InitTab();
        Debug.Log("Tab Box Init");
    }

    public void InitTab()
    {
        if(TabList.Count > 0)
        {
            for(int i = 0; i < TabList.Count; i++)
            {
                TabList[i].SetActiveTab(false);
            }

            TabList[SelectedIndex].SetActiveTab(true);
        }
    }

    public void DeleteTab(string name)
    {
        for(int i = 0; i < TabList.Count; i++)
        {
            if (TabList[i].GetName() == name)
            {
                TabList.RemoveAt(i);
                break;
            }
        }
    }

    public MyTab GetSelectedTab()
    {
        return TabList[SelectedIndex];
    }

    void AddAllObject(GameObject go, List<MyTab> list)
    {
        if (go.Get<MyTab>()!=null)
        {
            list.Add(go.Get<MyTab>());
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddAllObject(go.transform.GetChild(i).gameObject, list);
        }
    }
    
    public override bool AddControl(MyControl control)
    {
        if (control.GetControlType() != ControlType.Tab)
            return false;

        bool result = base.AddControl(control);
        if (result)
            TabList.Add((MyTab)control);

        return result;
    }

    public MyTab getTab(string name)
    {
        for (int i = 0; i < TabList.Count; i++)
        {
            if (TabList[i].GetName().Equals(name))
            {
                return TabList[i];
            }
        }

        return null;
    }

    public MyTab getTab(int idx)
    {
        if (idx >= 0 && idx < TabList.Count)
            return TabList[idx];
        else
            return null;
    }
}

