using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : Singleton<MainGameController>
{
    public enum GameType
    {
        Leapmotion1,
        Leapmotion2,
        Controller
    }
    public GameType gameType;
    public GameObject LearningContentCamera;
    public bool mIsFirst;
 
    private void Start()
    {
       //mIsFirst = true;
       //StartCoroutine(MainGameControll());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F1))
    //    {
    //       // NetworkMgr.Inst.glove;
    //    }
    //    if (Input.GetKeyDown(KeyCode.F2))
    //    {
    //    //    NetworkMgr.Inst.GlovesRightVibe();
    //    }
    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        NetworkMgr.Inst.GlovePairShock();
    //    }
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        change(pos[i++ % 4]);
    //    }
    //}

    string[] pos = { "[P]StartPosition (1)", "[P]StartPosition (2)", "[P]StartPosition (3)", "[P]StartPosition" };
    int i = 0;

    void change(string pos)
    {
        Transform camPos = PlayerMgr.Inst.Get("VRCamera").transform;
        PlayerMgr.Inst.SetCamera2Transform(camPos);

        PlayerMgr.Inst.SetPosition(pos);

        PlayerMgr.Inst.Get("RawImage").Get<Animation>()["New Animation"].time = 0;
        PlayerMgr.Inst.Get("RawImage").Get<Animation>().Play();
    }

    public IEnumerator MainGameControll()
    {
        //LearningContentCamera.SetActive(false);
        if (mIsFirst)
        {
            //NetworkMgr.Inst.Init();
        }
        mIsFirst = false;
        GameState.Inst.Init();
        PlayerMgr.Inst.PlayerInit();
        SoundMgr.Inst.InitSoundMgr();
  
        yield return StartCoroutine(GameState.Inst.LoadSceneAsync("TestScene"));
        
        PlayerMgr.Inst.InitOutlineObject();

        PlayerMgr.Inst.SetPosition("[P]First");
        if (gameType == GameType.Leapmotion1)
        {
            LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
            yield return StartCoroutine(GameState.Inst.StartGameMode("Loop"));
        }
        else if (gameType == GameType.Leapmotion2)
        {
            LeapHandMgr.Inst.ChangePlayerHand("Hand_Default");
            yield return StartCoroutine(GameState.Inst.StartGameMode("LoopGM_1"));
        }
        else
        {
            LeapHandMgr.Inst.ChangePlayerHand("NONE");
            yield return StartCoroutine(GameState.Inst.StartGameMode("ControllerGM"));
        }
        yield break;
    }
}