using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayIntroVideo : MonoBehaviour
{
    VideoPlayer introVideo;
    public GameObject LearningContent;
	
    private void Awake()
    {
        introVideo = GetComponent<VideoPlayer>();
        StartCoroutine(IntroPlay());
    }

    public IEnumerator IntroPlay()
    {
        LearningContent.SetActive(false);
        /*
        introVideo.Prepare();
        
        while (!introVideo.isPrepared)
            yield return null;
        
        introVideo.Play();
        while (introVideo.isPlaying)
            yield return null;
         */
        introVideo.targetCamera.gameObject.SetActive(false);
        StartCoroutine(MainGameController.Inst.MainGameControll());
        yield break;
    }
}
