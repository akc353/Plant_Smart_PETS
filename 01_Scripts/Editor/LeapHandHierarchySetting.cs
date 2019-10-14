using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;

public class LeapHandHierarchySetting : ScriptableWizard
{
    public GameObject fbx;
    public string LeftSign = "001";
    private string RightHandTag = "[RIGHT]";
    private string LeftHandTag = "[LEFT]";

    [MenuItem("MR/립모션/립모션 하이어라키 세팅")]
    static void CreateWizard()
    {
        LeapHandHierarchySetting windows = ScriptableWizard.DisplayWizard<LeapHandHierarchySetting>("Create Hand From FBX Hand Models", "Create");
        windows.minSize = new Vector2(500, 500);
    }

    void OnWizardCreate()
    {
        if (fbx == null)
        {
            Debug.LogError("FBX 참조가 없습니다. Scene에서 찾아 드래그 앤 드랍해주세요.");
            return;
        }

        var SkinnedMeshlist = fbx.Descendants(x => x.GetComponent<SkinnedMeshRenderer>() != null).ToList();

        if (SkinnedMeshlist.Count != 0)
        {
            Debug.LogError("Skinned Mesh Renderer를 찾을 수 없습니다. 메쉬를 다시 확인해주세요.");
            return;
        }

        if (SkinnedMeshlist.Count > 1)
        {
            Debug.LogError("Skinned Mesh Renderer는 하나여야 합니다. 메쉬를 다시 확인해주세요.");
            return;
        }

        var list = GetLeftAsset(fbx);

        GameObject LeftHand = new GameObject(LeftHandTag + fbx.name);
        list[0].transform.parent = LeftHand.transform;
        list[1].transform.parent = LeftHand.transform;

        GameObject RightHand = new GameObject(RightHandTag + fbx.name);
        fbx.transform.GetChild(0).parent = RightHand.transform;
        fbx.transform.GetChild(0).parent = RightHand.transform;

        MakeHand(LeftHand);
        MakeHand(RightHand);
    }

    public void MakeHand(GameObject ob)
    {
        // 구조 설정
        GameObject HandContainer = new GameObject("Palm");
        GameObject ForearmContainer = new GameObject("Forearm");

        var forearm = GetForearmPart(ob);
        var hand = forearm.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;

        hand.transform.parent = HandContainer.transform;
        forearm.transform.parent = ForearmContainer.transform;
        ForearmContainer.transform.parent = HandContainer.transform;
        HandContainer.transform.parent = ob.transform;

        // 손가락 세팅
        var fingerList = new List<Leap.Unity.RiggedFinger>();
        for (int i = 0; i < hand.transform.childCount; i++)
        {
            var riggedfinger = hand.transform.GetChild(i).gameObject.AddComponent<Leap.Unity.RiggedFinger>();

            if (i == 0)
            {
                riggedfinger.fingerType = Leap.Finger.FingerType.TYPE_THUMB;
            }
            if (i == 1)
            {
                riggedfinger.fingerType = Leap.Finger.FingerType.TYPE_INDEX;
            }
            if (i == 2)
            {
                riggedfinger.fingerType = Leap.Finger.FingerType.TYPE_MIDDLE;
            }
            if (i == 3)
            {
                riggedfinger.fingerType = Leap.Finger.FingerType.TYPE_RING;
            }
            if (i == 4)
            {
                riggedfinger.fingerType = Leap.Finger.FingerType.TYPE_PINKY;
            }

            riggedfinger.bones[1] = hand.transform.GetChild(i);
            riggedfinger.bones[2] = hand.transform.GetChild(i).GetChild(0);
            riggedfinger.bones[3] = hand.transform.GetChild(i).GetChild(0).GetChild(0);

            fingerList.Add(riggedfinger);
        }

        // 스크립트 세팅
        ob.AddComponent<Leap.Unity.HandEnableDisable>();
        var riggedhand = ob.AddComponent<Leap.Unity.RiggedHand>();

        var mg = new Leap.Unity.HandPool.ModelGroup();
        mg.GroupName = ob.name;
        riggedhand.group = mg;

        if (ob.name.Contains(RightHandTag))
        {
            riggedhand.Handedness = Leap.Unity.Chirality.Right;
            riggedhand.group.RightModel = ob.GetComponent<Leap.Unity.RiggedHand>();
        }
        if (ob.name.Contains(LeftHandTag))
        {
            riggedhand.Handedness = Leap.Unity.Chirality.Left;
            riggedhand.group.RightModel = ob.GetComponent<Leap.Unity.RiggedHand>();
        }

        riggedhand.palm = HandContainer.transform;
        riggedhand.forearm = ForearmContainer.transform;

        riggedhand.fingers[0] = fingerList[0];
        riggedhand.fingers[1] = fingerList[1];
        riggedhand.fingers[2] = fingerList[2];
        riggedhand.fingers[3] = fingerList[3];
        riggedhand.fingers[4] = fingerList[4];

        //var of = riggedhand.fingers[2].gameObject;
        //var mf = Instantiate<GameObject>(of);
        //mf.transform.parent = of.transform.parent;
        //mf.transform.localPosition = mf.transform.localPosition + mf.transform.up * 1f;

        //var result = mf.transform.localPosition - of.transform.localPosition;

        //Debug.Log(ob.name + " : " + result.x + " // " + result.y + " // " + result.z);
    }

    public GameObject GetForearmPart(GameObject ob)
    {
        for (int i = 0; i < ob.transform.childCount; i++)
        {
            var skin = ob.transform.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>();

            if (skin == null)
            {
                return ob.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }

    public List<GameObject> GetLeftAsset(GameObject ob)
    {
        var list = new List<GameObject>();

        for (int i = 0; i < ob.transform.childCount; i++)
        {
            if (ob.transform.GetChild(i).gameObject.name.Contains(LeftSign))
            {
                list.Add(ob.transform.GetChild(i).gameObject);
            }
        }

        return list;
    }
}