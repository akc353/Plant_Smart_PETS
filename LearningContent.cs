using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LearningContent : Singleton<LearningContent> {
  
    public GameObject VRCamera;
    public VideoPlayer mVideoPlayer;
  

    protected override void Awake()
    {
        StartCoroutine(PlayIntro());

    }

    public IEnumerator PlayIntro()
    {
        VRCamera.GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("Water");
        VRCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        VRCamera.GetComponent<Camera>().backgroundColor = Color.black;

        mVideoPlayer.gameObject.SetActive(true);
        mVideoPlayer.Prepare();

        while (!mVideoPlayer.isPrepared)
            yield return null;

        mVideoPlayer.Play();
        while (mVideoPlayer.isPlaying) yield return null;
       
        VRCamera.GetComponent<Camera>().cullingMask = -1;
        VRCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

        MainGameController.Inst.mIsFirst = true;
       

    }
}
