using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Linq;

public class FallingCrane : MonoBehaviour {
    public List<Animation> AnimationList;
    public Animation hydraulic;
    public Animation mast;
    public Animation mastPin;

    void Awake()
    {
        AnimationList = new List<Animation>();
        AnimationList = gameObject.GetComponentInAllChilds<Animation>();

        //for (int i = 0; i < AnimationList.Count; i++)
        //{
        //    if (AnimationList[i].gameObject.name.Equals("hydraulic assembly_AM_100m"))
        //        AnimationList.RemoveAt(i);
        //    else if (AnimationList[i].gameObject.name.Equals("Monorail_AM_100m"))
        //        AnimationList.RemoveAt(i);
        //    else if (AnimationList[i].gameObject.name.Equals("Monorail_pin_AM_100m"))
        //        AnimationList.RemoveAt(i);
        //}
        for (int i = AnimationList.Count - 1; i > -1; i--)
        {
            if (AnimationList[i].gameObject.name.Equals("hydraulic assembly_AM_100m"))
                AnimationList.RemoveAt(i);
            else if (AnimationList[i].gameObject.name.Equals("Monorail_AM_100m"))
                AnimationList.RemoveAt(i);
            else if (AnimationList[i].gameObject.name.Equals("Monorail_pin_AM_100m"))
                AnimationList.RemoveAt(i);
        }
        //SetCraneAnimation(false, 0);
    }

    public void PlayMastCombine()
    {
        mast.enabled = true;
        mast.clip = mast.GetClip("Take 001");
        
        mastPin.enabled = true;
        mastPin.clip = mastPin.GetClip("Take 001");
        //mast.clip = mast.GetClip("Take 001");
        mast.Play();

        //mast.clip = mast.GetClip("Take 001");
        mastPin.Play();
    }    
    public void SetCraneAnimation(bool isCollapse, int Cnt = 0)
    {
        if (!isCollapse)
        {
            for (int i = 0; i < AnimationList.Count; i++)
            {
                AnimationList[i].enabled = false;
            }

            return;
        }
        else if(Cnt == 0 || Cnt == 1)
        {
            for (int i = 0; i < AnimationList.Count; i++)
            {
                AnimationClip clip = null;
                AnimationList[i].enabled = true;
                //if (Cnt == 0)
                //    clip = AnimationList[i].GetClip("Take 001");
                if (Cnt == 1)
                {
                    clip = AnimationList[i].GetClip("Take 001 (1)");
                    
                }
                    AnimationList[i].clip = clip;
            }
            for (int i = 0; i < AnimationList.Count; i++)
            {
                if (AnimationList[i].clip != null)
                    AnimationList[i].Play();
            }

            if(Cnt == 1)
            {
                mast.clip = mast.GetClip("Take 001 (1)");
                mast.Play();

                mastPin.clip = mastPin.GetClip("Take 001 (1)");
                mastPin.Play();
            }
        }
        else
        {
            hydraulic.clip = hydraulic.GetClip("Take 001");
        
            hydraulic.enabled = true;
            hydraulic.Play();
        }
    }

    //public void SetCranePlay(bool isPlaying)
    //{
    //    if(isPlaying)
    //    {
    //        for (int i = 0; i < AnimationList.Count; i++)
    //        {
    //            if (AnimationList[i].name != null) {
    //                AnimationList[i][AnimationList[i].name].Reset();
    //                AnimationList[i].Play();
    //                AnimationList[i][AnimationList[i].name].SetTime(2);
    //                AnimationList[i].Play();
    //                Debug.Log("SetTime");
    //                }
    //            AnimationList[i].Play();
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < AnimationList.Count; i++)
    //            AnimationList[i].Stop();
    //    }
    //}
}
