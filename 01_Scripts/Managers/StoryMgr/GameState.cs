using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameState : Singleton<GameState>
{
    private Location nowLocation;
    public Location NowLocation
    {
        get
        {
            return nowLocation;
        }

        set
        {
            nowLocation = value;
        }
    }

    private Dictionary<string, GameMode> gameModeDic = new Dictionary<string, GameMode>();
    private Action PlayModePlayUpdate;
    public void Init()
    {
        for(int i = 0 ; i < this.gameObject.transform.childCount; i++)
        {
            var gm = this.gameObject.transform.GetChild(i).gameObject;

            gameModeDic.Add(gm.name, gm.GetComponent<GameMode>());
        }
    }

    private void Update()
    {
        if (PlayModePlayUpdate != null)
            PlayModePlayUpdate.Invoke();
    }


    public void SubscribePlayUpdate(Action  guestMethod)
    {
        PlayModePlayUpdate += guestMethod;
    }

    public void UnSubscribePlayUpdate(Action guestMethod)
    {
        PlayModePlayUpdate -= guestMethod;
    }



    public IEnumerator StartGameMode(string GameModeName)
    {
        return gameModeDic[GameModeName].StartGame();
    }

    public void LoadLocation(string locationName)
    {
        StartCoroutine(LoadSceneAsync(locationName));
    }

    public void UnLoadLocation(string locationName)
    {
        StartCoroutine(UnLoadSceneAsyne(locationName));
    }

    public IEnumerator LoadSceneAsync(string name)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        while (!ao.isDone)
        {
            Debug.Log("아직 로드되지 않음.");
            yield return null;
        }

        while (!SceneManager.SetActiveScene(SceneManager.GetSceneByName(name)))
        {
            Debug.Log("아직 활성화 되지 않음.");
            yield return null;
        }

        Debug.Log("활성화 완료.");

        NowLocation = GameObject.FindObjectOfType<Location>();
    }

    public IEnumerator UnLoadSceneAsyne(string name)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(name);

        while (!ao.isDone)
        {
            yield return null;
        }
    }
}