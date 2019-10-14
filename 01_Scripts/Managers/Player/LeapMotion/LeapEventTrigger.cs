using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapEventTrigger : MonoBehaviour
{
    [SerializeField] public string m_strEventName;

    public virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + " " + other.tag);

        if (other.gameObject.name.Equals("Contact Fingerbone") || other.gameObject.name.Equals("Contact Palm Bone"))  //test중이라 VRcam추가
        {
           // EventMgr._Inst.InvokeEvent(m_strEventName);
        }
    }
}