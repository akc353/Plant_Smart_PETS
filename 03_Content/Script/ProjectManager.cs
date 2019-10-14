using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectManager : MonoBehaviour {
    public GameObject LearningContentObj;
    Transform VROrigin;
    [Range(0f, 15f)]
    public float speed;


    private void Start()
    {
        VROrigin = PlayerMgr.Inst.Get("VROrigin").transform;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!LearningContentObj.activeInHierarchy)
                StartCoroutine(StopGame());
        }
        else if(Input.GetKey(KeyCode.KeypadPlus))
        {
           var pos = VROrigin.position;
           pos.y += Time.deltaTime * speed;

            VROrigin.position = pos;
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            var pos = VROrigin.position;
            pos.y -= Time.deltaTime * speed;

            VROrigin.position = pos;
        }
    }

    IEnumerator StopGame()
    {
        MainGameController.Inst.StopAllCoroutines();
        yield return StartCoroutine(GameState.Inst.UnLoadSceneAsyne("JIB"));

        StartCoroutine(MainGameController.Inst.MainGameControll());
    }
}
