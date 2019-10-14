using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class MyTab : MyControl
{
    private List<MyControl> Controls;

    public override void Init(string _name, ControlType _type = ControlType.Tab)
    {
        base.Init(_name, ControlType.Tab);
        if (!isInit)
        {
            Controls = new List<MyControl>();

            AddAllObject(gameObject, Controls);
            isInit = true;
        }
    }

    public void SetActiveTab(bool isOn)
    {
        if (!isInit)
            Init(name);
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().enabled = isOn;

        if (Controls == null)
            return;
        for(int i = 0; i < Controls.Count; i++)
        {
            Controls[i].gameObject.SetActive(isOn);
        }
    }

    void AddAllObject(GameObject go, List<MyControl> list)
    {
        if (go.Get<MyControl>() != null && go != gameObject)
        {
            list.Add(go.Get<MyControl>());
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddAllObject(go.transform.GetChild(i).gameObject, list);
        }
    }

    public override bool AddControl(MyControl control)
    {
        if (base.AddControl(control))
        {
            Controls.Add(control);
            return true;
        }

        return false;
    }

    public MyControl getTabControl(string name)
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

    public MyControl getTabControl(int idx)
    {
        if (idx >= 0 && idx < Controls.Count)
            return Controls[idx];
        else
            return null;
    }
}