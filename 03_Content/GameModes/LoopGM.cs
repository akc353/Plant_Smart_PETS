using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopGM : GameMode {
    public GameObject tabBox;

    void SelectTab(int idx, TabBox tabBox)
    {
        if (tabBox.SelectedIndex != idx)
        {
            tabBox.SelectedIndex = idx;
            tabBox.InitTab();
        }
    }

    IEnumerator PlayCD(MyButton button)
    {
        LeapHandMgr.Inst.ChangePlayerHand("NONE");
        Animation cdAnim = tabBox.transform.Find("PlayCD").gameObject.Get<Animation>();

        cdAnim.Play();
        yield return new WaitForSeconds(cdAnim.clip.length);

        button.Run();
        LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");

        MoveNextPoint();
    }

    IEnumerator ReturnCollider(bool isLeft, Collider col)
    {
        yield return new WaitForSeconds(0.3f);

        if (isLeft)
        {
            PlayerMgr.Inst.Get("[LEFT]Hand_Grip").Get<AutoDetect>().SubscribeEnterEvent(col.gameObject.name, () =>
            {
                Debug.Log("Success Left");
                PlayerMgr.Inst.Get("[LEFT]Hand_Grip").Get<AutoDetect>().Clean();
                LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
                MoveNextPoint();
            });

            PlayerMgr.Inst.Get("[LEFT]Hand_Grip").Get<AutoDetect>().SubscribeEnterEvent("RunIcon", () =>
            {
                StartCoroutine(PlayCD(col.gameObject.Get<MyButton>()));

                Debug.Log("Success Left");
                PlayerMgr.Inst.Get("[LEFT]Hand_Grip").Get<AutoDetect>().Clean();
                //LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
                //MoveNextPoint();
            });
        }
        else
        {
            PlayerMgr.Inst.Get("[RIGHT]Hand_Grip").Get<AutoDetect>().SubscribeEnterEvent(col.gameObject.name, () =>
            {
                Debug.Log("Success Right");
                PlayerMgr.Inst.Get("[RIGHT]Hand_Grip").Get<AutoDetect>().Clean();
                LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
                MoveNextPoint();
            });
            PlayerMgr.Inst.Get("[RIGHT]Hand_Grip").Get<AutoDetect>().SubscribeEnterEvent("RunIcon", () =>
            {
                StartCoroutine(PlayCD(col.gameObject.Get<MyButton>()));

                Debug.Log("Success Right");
                PlayerMgr.Inst.Get("[RIGHT]Hand_Grip").Get<AutoDetect>().Clean();
                //LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
                //MoveNextPoint();
            });
        }

        yield return StartCoroutine(CommonWaitPoint());

        yield return new WaitForSeconds(0.5f);
    }

    void InitTabBox()
    {
        GameObject obj;
        if (Get("TabBox_Small") == null)
            obj = Instantiate(tabBox, Get("Form").transform);
        else
            obj = Get("TabBox_Small");

        obj.SetActive(true);
        obj.name = "TabBox_Small";

        var pos = PlayerMgr.Inst.Get("VRCamera").transform.position;

        pos.y -= 0.1f;
        pos.z += 0.4f;

        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.06f, 0.06f);

        pos.y += 0.25f;

        Get("RunIcon").transform.SetParent(Get("Form").transform);
        Get("RunIcon").transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        Get("RunIcon").SetActive(true);
        Get("RunIcon").transform.position = pos;

        PlayerMgr.Inst.Get("RightHand").SetActive(false);
        PlayerMgr.Inst.Get("LeftHand").SetActive(false);

        tabBox = obj;
    }

    public override IEnumerator StartGame()
    {
        //TabBox 초기화
        InitTabBox();

        do
        {
            ControlType type = ControlType.None;
            bool isLeft = false;

            LeapHandMgr.Inst.AddAllLeftFingerDetection("Button", () =>
             {
                 isLeft = true;
                 type = ControlType.Button;

                 LeapHandMgr.Inst.CleanAllFingerDetection();
                 MoveNextPoint();
             });
            LeapHandMgr.Inst.AddAllRightFingerDetection("Button", () =>
            {
                isLeft = false;
                type = ControlType.Button;

                LeapHandMgr.Inst.CleanAllFingerDetection();
                MoveNextPoint();
            });

            LeapHandMgr.Inst.AddAllLeftFingerDetection("Tab", () =>
            {
                isLeft = true;
                type = ControlType.Tab;

                LeapHandMgr.Inst.CleanAllFingerDetection();
                MoveNextPoint();
            });
            LeapHandMgr.Inst.AddAllRightFingerDetection("Tab", () =>
            {
                isLeft = false;
                type = ControlType.Tab;

                LeapHandMgr.Inst.CleanAllFingerDetection();
                MoveNextPoint();
            });
            yield return StartCoroutine(CommonWaitPoint());


            if(type == ControlType.Button)
            {
                //손바꾸기

                //몇초이상들고있으면 선택
                //손다시 닿았던데에 닿으면 원래 손으로 돌아가고
                if (isLeft)
                {
                    LeapHandMgr.Inst.ChangePlayerHand("[LEFT]Hand_Grip", "[RIGHT]Hand_Default");
                    yield return ReturnCollider(isLeft, LeapHandMgr.Inst.leftCol);
                }
                else
                {
                    LeapHandMgr.Inst.ChangePlayerHand("[LEFT]Hand_Default", "[RIGHT]Hand_Grip");
                    yield return ReturnCollider(isLeft, LeapHandMgr.Inst.rightCol);
                }
            }
            else if(type == ControlType.Tab)
            {
                if (isLeft)
                    SelectTab(LeapHandMgr.Inst.leftCol.transform.GetSiblingIndex(), LeapHandMgr.Inst.leftCol.transform.parent.gameObject.Get<TabBox>());
                else
                    SelectTab(LeapHandMgr.Inst.rightCol.transform.GetSiblingIndex(), LeapHandMgr.Inst.rightCol.transform.parent.gameObject.Get<TabBox>());
            }

            LeapHandMgr.Inst.leftCol = null;
            LeapHandMgr.Inst.rightCol = null;
        } while (true);
    }
}
