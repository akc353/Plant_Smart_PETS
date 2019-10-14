using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerMgr : MonoBehaviour {
    public Dictionary<ControlType, GameObject> ControlArray;

    //컨테이너가 가진 컨트롤의 이름에 따라 컨트롤타입 반환
    ControlType GetControlType(string name)
    {
        switch (name)
        {
            case "Tab":
                return ControlType.Tab;
            case "Button":
                return ControlType.Button;
            case "Label":
                return ControlType.Label;
            case "Image":
                return ControlType.Image;
            case "TabBox":
                return ControlType.TabBox;
            case "None":
                return ControlType.None;
            default:
                return ControlType.Exception;
        }
    }
    
    //컨트롤 타입에 맞는 게임오브젝트 반환
    public GameObject GetControl(ControlType controlType)
    {
        if (controlType == ControlType.Exception)
            return null;
        
        return Instantiate(ControlArray[controlType]);
    }


    private void Start()
    {
        ControlArray = new Dictionary<ControlType, GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            ControlType ct = GetControlType(g.name);

            if (ct != ControlType.Exception)
            {
                ControlArray.Add(ct, g);
            }
        }
    }
}
