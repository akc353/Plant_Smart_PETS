using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayshoot : MonoBehaviour
{
    public bool triggerUp = false;

    public bool trigger = false;
    public System.Action action = null;
    public System.Action collisionAction = null;

    int mask1;
    int mask2;
    int mask3;

    bool 작동 = true;

    public Transform ray;

    private void Start()
    {
        mask1 = 1 << LayerMask.NameToLayer("Tab");
        mask2 = 1 << LayerMask.NameToLayer("Button");
        mask3 = 1 << LayerMask.NameToLayer("RunIcon");
    }

    void SelectTab(int idx, TabBox tabBox)
    {
        if (tabBox.SelectedIndex != idx)
        {
            tabBox.SelectedIndex = idx;
            tabBox.InitTab();
        }
    }

    void SetRayDistance(float z)
    {
        ray.localScale = new Vector3(ray.localScale.x, ray.localScale.y, z);
    }

    IEnumerator PlayCD(string name)
    {
        작동 = false;
        Animation cdAnim = GameObject.Find("TabBox_Obj").transform.Find("PlayCD").gameObject.Get<Animation>();

        cdAnim.Play();
        yield return new WaitForSeconds(cdAnim.clip.length);
        //ReadFile.Inst.buttonPath[name];
        Debug.Log(name);
        if (ReadFile.Inst.buttonPath.ContainsKey(name))
        {
            Debug.Log(ReadFile.Inst.buttonPath[name]);
            System.Diagnostics.Process.Start(ReadFile.Inst.buttonPath[name]);
            //.Log(name);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
        }
        작동 = true;
    }

    void Update()
    {
        if (작동)
        {
            if (trigger)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask1))
                {
                    SetRayDistance(hit.distance);
                    SelectTab(hit.collider.transform.GetSiblingIndex(), hit.transform.parent.gameObject.Get<TabBox>());
                }
                else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask2))
                {
                    SetRayDistance(hit.distance);
                }
                else
                {
                    SetRayDistance(20f);
                }
            }
            else if (triggerUp)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask3))
                {
                    StartCoroutine(PlayCD(hit.collider.gameObject.name.Split('_')[0]));
                    //hit.collider.gameObject.Get<MyButton>().Run(); //얘는 당기는거 풀면서 동작하도록
                }
                else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask3))
                {
                    StartCoroutine(PlayCD(hit.collider.gameObject.name.Split('_')[0]));
                    //hit.collider.gameObject.Get<MyButton>().Run(); //얘는 당기는거 풀면서 동작하도록
                }
                else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI")))
                {
                    StartCoroutine(PlayCD(hit.collider.gameObject.name.Split('_')[0]));
                    //hit.collider.gameObject.Get<MyButton>().Run(); //얘는 당기는거 풀면서 동작하도록
                }
                triggerUp = false;
            }
            else
            {
                SetRayDistance(0f);
            }
        }
    }
}
