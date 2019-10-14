using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum HandSide
{
    LEFT,
    RIGHT
}

public class FingerDetection : MonoBehaviour
{
    public HandSide _side;
    public Dictionary<string, System.Action> enterEventDic = new Dictionary<string, System.Action>();
    public Dictionary<string, System.Action> exitEventDic = new Dictionary<string, System.Action>();

    public void SubscribeEnterEvent(string name, System.Action action)
    {
        enterEventDic.Add(name, action);
    }

    public void UnSubscribeEnterEvent(string name)
    {
        enterEventDic.Remove(name);
    }

    public void SubscribeExitEvent(string name, System.Action action)
    {
        exitEventDic.Add(name, action);
    }

    public void UnSubscribeExitEvent(string name)
    {
        exitEventDic.Remove(name);
    }

    public void OnCollisionEnter(Collision collision)
    {
        List<string> list = enterEventDic.Keys.ToList();

        foreach (string name in list)
        {
            if (collision.gameObject.name.Equals(name) || collision.gameObject.tag.Equals(name) || collision.gameObject.layer == LayerMask.NameToLayer(name))
            {
                if (gameObject.tag.Equals("[RIGHT]Finger"))
                    LeapHandMgr.Inst.rightCol = collision.collider;
                else if (gameObject.tag.Equals("[LEFT]Finger"))
                    LeapHandMgr.Inst.leftCol = collision.collider;
                enterEventDic[name].Invoke();
                enterEventDic.Remove(name);
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        List<string> list = enterEventDic.Keys.ToList();

        foreach (string name in list)
        {
            if (collision.gameObject.name.Equals(name) || collision.gameObject.tag.Equals(name) || collision.gameObject.layer == LayerMask.NameToLayer(name))
            {
                enterEventDic[name].Invoke();
                enterEventDic.Remove(name);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        List<string> list = enterEventDic.Keys.ToList();

        foreach (string name in list)
        {
            if (other.gameObject.name.Equals(name) || other.gameObject.tag.Equals(name) || other.gameObject.layer == LayerMask.NameToLayer(name))
            {
                if (gameObject.tag.Equals("[RIGHT]Finger"))
                    LeapHandMgr.Inst.rightCol = other;
                else if (gameObject.tag.Equals("[LEFT]Finger"))
                    LeapHandMgr.Inst.leftCol = other;
                
                enterEventDic[name].Invoke();
                enterEventDic.Remove(name);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        List<string> list = exitEventDic.Keys.ToList();

        foreach (string name in list)
        {
            if (other.gameObject.name.Equals(name) || other.gameObject.tag.Equals(name) || other.gameObject.layer == LayerMask.NameToLayer(name))
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