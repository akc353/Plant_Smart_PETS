using cakeslice;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using Leap.Unity.Interaction;

public class GameMode : MonoBehaviour
{
    public virtual IEnumerator StartGame()
    {
        yield return null;
    }

    #region 멀티 업데이트
    public List<DynamicUpdate> ucList = new List<DynamicUpdate>();

    public void MultiUpdate(System.Func<bool> condition, System.Action callback, string name, bool autoDisconnect = true)
    {
        DynamicUpdate CheckTouchWrap = new DynamicUpdate(name, autoDisconnect, condition, callback);
        ucList.Add(CheckTouchWrap);
        Debug.Log(name + " Update 등록 (현재 : " + ucList.Count + " 개)");

        foreach (DynamicUpdate uc in ucList)
        {
            Debug.Log(uc.name);
        }
    }



    public void RemoveMultiUpdate(string name)
    {
        foreach (DynamicUpdate uc in ucList)
        {
            if (uc.name == name)
            {
                uc.DisConnect();
                ucList.Remove(uc);
            }
        }
    }

    public void AllRemoveMultiUpdate()
    {
        foreach (DynamicUpdate uc in ucList)
        {
            uc.DisConnect();
        }
        ucList.Clear();
    }
    #endregion

    #region 스토리 제어
    private bool _commonWait = false;

    public IEnumerator CommonWaitPoint()
    {
        _commonWait = true;

        while (_commonWait)
        {
            yield return null;
        }
    }

    public void MoveNextPoint()
    {
        _commonWait = false;
    } 
    #endregion

    public GameObject Get(string name)
    {
        return GameState.Inst.NowLocation.Get(name);
    }
}