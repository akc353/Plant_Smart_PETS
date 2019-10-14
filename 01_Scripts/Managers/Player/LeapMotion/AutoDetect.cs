using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AutoDetect : MonoBehaviour
{
    public Dictionary<string, System.Action> enterEventDic = new Dictionary<string, System.Action>();
    public Dictionary<string, System.Action> exitEventDic = new Dictionary<string, System.Action>();

    public void SubscribeEnterEvent(string name, System.Action action)
    {
        enterEventDic.Add(name, action);

        Debug.Log(name + " : 이벤트 등록됨 총갯수 -> " + enterEventDic.Count);
    }

    public void UnSubscribeEnterEvent(string name)
    {
        enterEventDic.Remove(name);

        Debug.Log(name + " : 이벤트 제거됨 총갯수 -> " + enterEventDic.Count);
    }

    public void SubscribeExitEvent(string name, System.Action action)
    {
        exitEventDic.Add(name, action);
    }

    public void UnSubscribeExitEvent(string name)
    {
        exitEventDic.Remove(name);
    }

    public void OnTriggerEnter(Collider other)
    {
        List<string> list = enterEventDic.Keys.ToList();

        Debug.Log(other.name + " " + list.Count );

        foreach (string name in list)
        {
            Debug.Log(other.name + " " + name);
            if (other.gameObject.name.Equals(name))
            {
                enterEventDic[name].Invoke();
                enterEventDic.Remove(name);
            }
            if (other.gameObject.tag.Equals(name))
            {
                enterEventDic[name].Invoke();
                enterEventDic.Remove(name);
            }
        }
    }


    //public void OnTriggerStay(Collider other)
    //{
    //    List<string> list = enterEventDic.Keys.ToList();

    //    Debug.Log(other.name + " " + list.Count );

    //    foreach (string name in list)
    //    {
    //        Debug.Log(other.name + " " + name);
    //        if (other.gameObject.name.Equals(name))
    //        {
    //            enterEventDic[name].Invoke();
    //            enterEventDic.Remove(name);
    //        }
    //        if (other.gameObject.tag.Equals(name))
    //        {
    //            enterEventDic[name].Invoke();
    //            enterEventDic.Remove(name);
    //        }
    //    } 
    //}


    public void OnTriggerExit(Collider other)
    {
        List<string> list = exitEventDic.Keys.ToList();

        foreach (string name in list)
        {
            if (other.gameObject.name.Equals(name))
            {
                exitEventDic[name].Invoke();
                exitEventDic.Remove(name);
            }
            if (other.gameObject.tag.Equals(name))
            {
                exitEventDic[name].Invoke();
                exitEventDic.Remove(name);
            }
        }
    }

    public void Clean()
    {
        enterEventDic.Clear();
        exitEventDic.Clear();
    }
}