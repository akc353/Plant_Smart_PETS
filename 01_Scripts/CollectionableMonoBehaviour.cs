using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionableMonoBehaviour : MonoBehaviour
{
    protected bool isCollect = false;
    private Dictionary<string, GameObject> AllChildOfThisObject = new Dictionary<string, GameObject>();

    public void OnEnable()
    {
        if (!isCollect)
        {
            AddAllObjectInLocation(this.gameObject, AllChildOfThisObject);

            isCollect = true;
        }
    }

    void AddAllObjectInLocation(GameObject go, Dictionary<string, GameObject> dic)
    {
        if (!dic.ContainsKey(go.name))
        {
            dic.Add(go.name, go);
        }

        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddAllObjectInLocation(go.transform.GetChild(i).gameObject, dic);
        }
    }

    void AddObject(GameObject go)
    {
        if (AllChildOfThisObject.ContainsKey(go.name))
        {
            Debug.Log(go.name + " : 같은 이름의 오브젝트가 이미 있습니다.");
        }
        else
        {
            AllChildOfThisObject.Add(go.name, go);
        }
    }

    void AddObject(GameObject go, Dictionary<string, GameObject> dic)
    {
        if (dic.ContainsKey(go.name))
        {
            Debug.Log(go.name + " : 같은 이름의 오브젝트가 이미 있습니다.");
        }
        else
        {
            dic.Add(go.name, go);
        }
    }

    public void RemoveObject(string name)
    {
        if (AllChildOfThisObject.ContainsKey(name))
        {
            AllChildOfThisObject.Remove(name);
        }
    }

    public GameObject Get(string gameObjectName)
    {
        if (AllChildOfThisObject.ContainsKey(gameObjectName))
        {
            return AllChildOfThisObject[gameObjectName];
        }
        else
        {
            Debug.Log("[" + gameObjectName + "] : 이름 확인 하세요");
            return null;
        }
    }

    public GameObject CopyGameObject(string ObjectName, string CloneName)
    {
        var original = Get(ObjectName);

        GameObject clone = Instantiate(original, original.transform.position, original.transform.rotation);
        clone.transform.parent = original.transform.parent;
        clone.name = CloneName;
        clone.SetActive(true);
        AddObject(clone);

        return clone;
    }
}