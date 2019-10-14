using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

//앞으로 계속해서 추가될 립모션 이벤트 추가를 위해서 립모션 매니저를 따로 뺐습니다.
//립모션 전용
public class LeapHandMgr : Singleton<LeapHandMgr>
{
    public CollectionableMonoBehaviour rootCollection;
    public GameObject mCopyHandL, mCopyHandR;
    public RiggedHand mNowRightHand, mNowLeftHand;
    public Material mChangeMatL, mChangeMatR;    //손 매테리얼 바꿀시
    public Material mOriginMatL, mOriginHandR;

    public Collider leftCol = null;
    public Collider rightCol = null;

    private void Update()
    {
#if UNITY_EDITOR
        //손 복제
        if (Input.GetKeyDown(KeyCode.Delete)) //왼손
        {
            InstantiateHand(GetLeftHand(1));
        }
        else if (Input.GetKeyDown(KeyCode.End)) //오른손
        {
            InstantiateHand(GetRightHand(1));
        }
#endif
    }



    public GameObject Get(string name)
    {
        if (rootCollection == null)
        {
            rootCollection = PlayerMgr.Inst.rootCollection;
        }

        return rootCollection.Get(name);
    }

    public GameObject GetRightHand(int i = -1)
    {
        if (i == -1)
        {
            return mNowRightHand.gameObject;
        }
        else
        {
            return mNowRightHand.fingers[i].bones[3].GetChild(0).gameObject;
        }
    }

    public GameObject GetLeftHand(int i = -1)
    {
        if (i == -1)
        {
            return mNowLeftHand.gameObject;
        }
        else
        {
            return mNowLeftHand.fingers[i].bones[3].GetChild(0).gameObject;
        }
    }

    public void ChangeLeftHand(string leftHand)
    {
        var pool = Get("LeapHandController").Get<HandPool>();

        if (leftHand == "NONE")
        {
            mNowLeftHand = null;
        }
        else
        {
            pool.EnableGroup(leftHand);
            mNowLeftHand = GetHand(leftHand);
        }
    }

    public void ChangeRightHand(string rightHand)
    {
        var pool = Get("LeapHandController").Get<HandPool>();

        if (rightHand == "NONE")
        {
            mNowRightHand = null;
        }
        else
        {
            pool.EnableGroup(rightHand);
            mNowRightHand = GetHand(rightHand);
        }
    }


    public void ChangePlayerHand(string handName)
    {
        var pool = Get("LeapHandController").Get<HandPool>();

        foreach (var mg in pool.ModelPool)
        {
            pool.DisableGroup(mg.GroupName);
        }

        if (handName == "NONE")
        {
            mNowLeftHand = null;
            mNowRightHand = null;
        }
        else
        {
            ChangeLeftHand("[LEFT]" + handName);
            ChangeRightHand("[RIGHT]" + handName);
        }
    }

    public void ChangePlayerHand(string pLeftName, string pRightName)
    {
        var pool = Get("LeapHandController").Get<HandPool>();

        foreach (var mg in pool.ModelPool)
        {
            pool.DisableGroup(mg.GroupName);
        }

        if (pLeftName.Contains("[LEFT]") || pRightName.Contains("RIGHT"))
        {
            ChangeLeftHand(pLeftName);
            ChangeRightHand(pRightName);
        }
        else
        {
            ChangeLeftHand(pRightName);
            ChangeRightHand(pLeftName);
        }
    }


    public RiggedHand GetHand(string name)
    {
        var pool = Get("LeapHandController").Get<HandPool>();

        foreach (var mg in pool.ModelPool)
        {
            if (mg.GroupName == name)
            {
                if (mg.GroupName.Contains("[RIGHT]"))
                {
                    return mg.RightModel.gameObject.Get<RiggedHand>();
                }
                else
                {
                    return mg.LeftModel.gameObject.Get<RiggedHand>();
                }
            }
        }

        Debug.Log("입력한 이름의 손이 없습니다. Null 리턴!");

        return null;
    }

    // 손 모델을 복제하여 손가락의 움직임을 해제합니다.
    public void InstantiateHand(GameObject hand)
    {
        var copyHand = Instantiate<GameObject>(hand);

        AllChildRendererSetEnabled(copyHand, true);
        copyHand.SetActive(true);

        copyHand.transform.Find("Palm").position = hand.transform.Find("Palm").position;
        copyHand.transform.Find("Palm").rotation = hand.transform.Find("Palm").rotation;

        copyHand.Get<Leap.Unity.RiggedHand>().fingers = new Leap.Unity.FingerModel[5];
    }

    // 랜더러의 상태 세팅 메소드
    public void AllChildRendererSetEnabled(GameObject ob, bool value)
    {
        SkinnedMeshRenderer[] rends = ob.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int ri = 0; ri < rends.Length; ri++)
            rends[ri].enabled = value;
    }

    //손 전체 꺼줍니다.
    public void AllDisableHand()
    {
        List<string> handNameList = new List<string>();
        var pool = Get("LeapHandController").Get<HandPool>();

        int handIdx = 0;

        foreach (var mg in pool.ModelPool)
        {
            handNameList.Add(mg.GroupName);
            pool.DisableGroup(handNameList[handIdx]);
            handIdx++;
        }

        handNameList.Clear();
    }


    public void CleanAllFingerDetection()
    {
        for (int i = 0; i < 5; i++)
        {
            GetLeftHand(i).Get<FingerDetection>().Clean();
            GetRightHand(i).Get<FingerDetection>().Clean();
        }
    }

    public void AddAllLeftFingerDetection(string name, System.Action action)
    {
        for (int i = 0; i < 5; i++)
        {
            action += () =>
            {
                for (int j = 0; j < 5; j++)
                    GetLeftHand(j).Get<FingerDetection>().Clean();
            };
            GetLeftHand(i).Get<FingerDetection>().SubscribeEnterEvent(name, action);
        }
    }


    public void AddAllRightFingerDetection(string name, System.Action action)
    {
        for (int i = 0; i < 5; i++)
        {
            action += () =>
            {
                for (int j = 0; j < 5; j++)
                    GetRightHand(j).Get<FingerDetection>().Clean();
            };
            GetRightHand(i).Get<FingerDetection>().SubscribeEnterEvent(name, action);
        }
    }
}
