using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType{
    Exception = -1,
    None,
    Button,
    Label,
    Image,
    TabBox,
    Tab,
}

public class WindowMgr : Singleton<WindowMgr> {
    //현재 WindowMgr의 자식으로 추가된 Contorl의 게임오브젝트 리스트
    public Dictionary<string, GameObject> objectList;

    public WindowCtrl winCtrl;
    //컨트롤 종류를 가지고 있는 컨테이너
    public ContainerMgr container;
    

    void Start()
    {
        //objectList 초기화
        objectList = new Dictionary<string, GameObject>();

        //Button Path 읽어오기
        ReadFile.Inst.ReadButtonPath();
        AddAllObject(gameObject, objectList);
    }
    

    //자식으로 가지고 있는 컨트롤을 objectList를 추가
    void AddAllObject(GameObject go, Dictionary<string, GameObject> dic)
    {
        if (!dic.ContainsKey(go.name))
        {
            dic.Add(go.name, go);
            MyControl mc = go.Get<MyControl>();
            
            if (mc.GetType() == (typeof(TabBox)))
                mc = go.Get<TabBox>();
            else if(mc.GetType() == (typeof(MyTab)))
                mc = go.Get<MyTab>();
            else if(mc.GetType() == (typeof(MyButton)))
                mc = go.Get<MyButton>();

            mc.Init(go.name, mc.GetControlType());
            winCtrl.AddControl(mc);
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddAllObject(go.transform.GetChild(i).gameObject, dic);
        }
    }

    //objectList에 컨트롤 추가
    public void AddControl(MyControl mc, ControlType controlType)
    {
        GameObject newObj = container.GetControl(controlType);

        newObj.GetComponent<MyControl>().Init(newObj.name, controlType);

        if (winCtrl.AddControl(newObj.GetComponent<MyControl>())) {
            MyControl newOne = newObj.GetComponent<MyControl>();

            newObj.transform.SetParent(mc.transform);
            newObj.transform.localPosition = Vector3.zero;

            objectList.Add(newObj.name, newObj);
        }
        else
        {
            Destroy(newObj, 0);
        }
    }

    public GameObject GetObj(string name)
    {
        if (objectList.ContainsKey(name))
            return objectList[name];
        return null;
    }

    public void DeleteTab(string name)
    {
        gameObject.Get<TabBox>().DeleteTab(name);
    }

    public void DeleteControl_Recursion(MyControl mc)
    {
        for(int i = 0; i < mc.transform.childCount; i++)
        {
            DeleteControl_Recursion(mc.transform.GetChild(i).gameObject.Get<MyControl>());
        }

        winCtrl.DeleteControl(mc);
        objectList.Remove(mc.GetName());

        Destroy(mc.gameObject);
    }

    public void DebugObjList()
    {
       foreach(var dic in objectList)
        {
            Debug.Log(dic.Key + " : " + dic.Value);
        }
        Debug.Log(objectList.Count);
    }
    
    public void DeleteControl(MyControl control)
    {
        if(GetObj(control.GetName()) != null)
        {
            if(control.GetControlType() == ControlType.Tab)
            {
                DeleteTab(control.GetName());
            }
            
            DeleteControl_Recursion(control);
            DebugObjList();
        }
    }

    //objectList에 있는 게임오브젝트의 이름과 해당 키 값 동기화
    public void objListSetName()
    {
        foreach (var obj in objectList)
        {
            if (!obj.Key.Equals(obj.Value.name))
            {
                if (objectList.ContainsKey(obj.Value.name))
                {
                    obj.Value.name = obj.Key;
                }
                else
                {
                    objectList.Remove(obj.Key);

                    objectList.Add(obj.Value.name, obj.Value);
                }
            }
            Debug.Log(obj.Key + " : " + obj.Value + " : " + obj.Value.Get<MyControl>().GetName());
        }
    }

    //Init 버튼 누를 시 호출
    //WindowMgr의 objectList와 WindowCtrl의 Controls 이름 초기화
    public void SetName()
    {
        objListSetName();
        winCtrl.SetName();
    }
}
