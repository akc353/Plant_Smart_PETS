using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LeapHandTranformSetting : ScriptableWizard
{
    public Transform 기존_베이스_모델;
    public Transform 커스텀_모델;

    private List<Transform> baseList;
    private List<Transform> customList;

    private Vector3 fingerPoint;
    private Vector3 facing;

    [MenuItem("MR/립모션/립모션 트랜스폼 세팅")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<LeapHandTranformSetting>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        baseList = new List<Transform>();
        customList = new List<Transform>();

        fingerPoint = 기존_베이스_모델.GetComponent<Leap.Unity.RiggedHand>().modelFingerPointing;
        facing = 기존_베이스_모델.GetComponent<Leap.Unity.RiggedHand>().modelPalmFacing;

        커스텀_모델.GetComponent<Leap.Unity.RiggedHand>().modelFingerPointing = fingerPoint;
        커스텀_모델.GetComponent<Leap.Unity.RiggedHand>().modelPalmFacing = facing;
        커스텀_모델.GetComponent<Leap.Unity.RiggedHand>().SetEditorLeapPose = false;
        커스텀_모델.GetComponent<Leap.Unity.RiggedHand>().ModelPalmAtLeapWrist = false;

        var list = 커스텀_모델.GetComponent<Leap.Unity.RiggedHand>().fingers;
        foreach(Leap.Unity.RiggedFinger f in list)
        {
            f.modelFingerPointing = fingerPoint;
            f.modelPalmFacing = facing;
        }

        if (기존_베이스_모델 != null)
        {
            AllChildObject(기존_베이스_모델, baseList);
        }
        if (커스텀_모델 != null)
        {
            AllChildObject(커스텀_모델, customList);
        }

        for (int i = 0; i< baseList.Count; i ++)
        {
            customList[i].localPosition = baseList[i].localPosition;
            customList[i].localRotation = baseList[i].localRotation;
            customList[i].localScale = baseList[i].localScale;
        }
    }

    public void AllChildObject(Transform motherTrans, List<Transform> list)
    {
        list.Add(motherTrans);

        if (motherTrans.childCount != 0)
        {
            for (int i = 0; i < motherTrans.childCount; i++)
            {
                AllChildObject(motherTrans.GetChild(i), list);
            }
        }
    }
}