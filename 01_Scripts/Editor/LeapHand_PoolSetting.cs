using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using UnityEditor;

public class LeapHandPool_Setting : MonoBehaviour
{
    [MenuItem("MR/립모션/립 Hand Pool 세팅")]
    public static void PlayerHandInit()
    {
        var pool = Get("LeapHandController").Get<HandPool>();
        pool.ModelPool = new List<HandPool.ModelGroup>();

        var hands = Get("Hands");
        var allhands = hands.GetChilds();

        foreach (var ob in allhands)
        {
            var mg = new HandPool.ModelGroup();
            mg.GroupName = ob.name;

            if (ob.name.Contains("[RIGHT]"))
            {
                mg.RightModel = ob.Get<RiggedHand>();

                var dic = ob.GetAllChilds();

                SetFingerComponent(dic["Bip01 R Finger0Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger1Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger2Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger3Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger4Nub"], HandSide.RIGHT, "[RIGHT]Finger");
            }
            else
            {
                mg.LeftModel = ob.Get<RiggedHand>();

                var dic = ob.GetAllChilds();

                SetFingerComponent(dic["Bip01 R Finger0Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger1Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger2Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger3Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger4Nub001"], HandSide.LEFT, "[LEFT]Finger");
            }

            if (ob.name.Contains("Default"))
            {
                mg.IsEnabled = true;
            }
            else
            {
                mg.IsEnabled = false;
            }

            pool.ModelPool.Add(mg);
        }
    }

    public static void SetFingerComponent(GameObject finger, HandSide side, string tag)
    {
        var c = finger.Get<Collider>();
        if (c != null)
        {
            GameObject.DestroyImmediate(c);
        }

        var i = finger.Get<FingerDetection>();
        if (i != null)
        {
            GameObject.DestroyImmediate(i);
        }

        var r = finger.Get<Rigidbody>();
        if (r != null)
        {
            GameObject.DestroyImmediate(r);
        }

        var ff = finger.AddComponent<SphereCollider>();
        ff.radius = 0.02f;
        ff.isTrigger = true;

        var fd = finger.AddComponent<FingerDetection>();
        fd._side = side;

        var fr = finger.AddComponent<Rigidbody>();
        fr.isKinematic = true;
        fr.useGravity = false;

        finger.tag = tag;
    }

    static GameObject Get(string name)
    {
        return GameObject.Find(name);
    }
}
