using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using Leap.Unity;
using Leap.Unity.Interaction;
using PostProcess;
using cakeslice;

public class PlayerMgr : Singleton<PlayerMgr>
{
    public CollectionableMonoBehaviour rootCollection;
    public Camera playerCamera;
    public CameraFilterController mCameraFilterController;
    public Dictionary<string, Transform> positionDic = new Dictionary<string, Transform>();
    public Dictionary<string, CPC_CameraPath> pathDic = new Dictionary<string, CPC_CameraPath>();
    private Dictionary<string, cakeslice.Outline> mOutlineObjDic = new Dictionary<string, cakeslice.Outline>();
    private bool mIsSetTime = false;
    public float mSetTimeScale = 7f;
    public RiggedHand NowRightHand, NowLeftHand;
    
    public Camera Camera2;
    
    public void SetCamera2Transform(Transform trans)
    {
        Camera2.transform.position = trans.position;
        Camera2.transform.rotation = trans.rotation;
    }

    //public Coroutine FadeCamera(float time)
    //{
    //    return StartCoroutine(co_FadeCamera(time));
    //}
    //
    //IEnumerator co_FadeCamera(float time)
    //{
    //    mCameraFilterController.StartBlend(time);
    //    yield return new WaitForSeconds(time);
    //}

    public void PlayerInit()
    {
        PlayerHeadBobInit();
        PlayerUIInit();
        BlinkInit();
        mCameraFilterController = this.transform.Find("VRCamera").GetComponent<CameraFilterController>();
        mCameraFilterController.ResetFilter();
    }

    private void Update()
    {  
        //타임갑 원래속도, 빨리감기 토글
        if (Input.GetKeyDown(KeyCode.F5))
        {
            mIsSetTime = !mIsSetTime;
            if (mIsSetTime)
                Time.timeScale = mSetTimeScale;
            else
                Time.timeScale = 1f;
        }
    }

    #region Set OutLine 
    //씬 이동후 켜져있는 오브젝트에 아웃라인이 있다면 아웃라인 딕셔너리에 싹 담습니다.
    public void InitOutlineObject()
    {
        cakeslice.Outline[] outlines = FindObjectsOfType<cakeslice.Outline>();

        for (int i = 0; i < outlines.Length; i++)
        {
            if (string.IsNullOrEmpty(outlines[i].gameObject.name))
            {
                continue;
            }

            mOutlineObjDic.Add(outlines[i].gameObject.name, outlines[i]);
            mOutlineObjDic[outlines[i].gameObject.name].enabled = false;
            mOutlineObjDic[outlines[i].gameObject.name].eraseRenderer = true;
        }
    }


    //아웃라인 true,false
    public void SetOutline(string pGoName, bool pSet)
    {
        foreach(KeyValuePair<string, cakeslice.Outline> pair in mOutlineObjDic)
        {
            if (pSet)
            {
                mOutlineObjDic[pGoName].enabled = true;
                mOutlineObjDic[pGoName].eraseRenderer = false;
            }
            else
            {
                mOutlineObjDic[pGoName].enabled = false;
                mOutlineObjDic[pGoName].eraseRenderer = true;

            }
        }  
    }

    //현재 씬에 켜져있는 모든 아웃라인  off
    public void AllOffOutlines()
    {
        cakeslice.Outline[] outlines = FindObjectsOfType<cakeslice.Outline>();
        for (int i = 0; i < outlines.Length; i++)
        {
            if (string.IsNullOrEmpty(outlines[i].gameObject.name))
            {
                continue;
            }

            outlines[i].enabled = false;
            outlines[i].eraseRenderer = true; 
        }
    }

    #endregion
 
   

    #region UI


    public void PlayerUIInit()
    {
        //ChangeMaterialWithAllChild(this.transform, Resources.Load<Material>("3DUIMaterial/UI_Default_OverlayNoZTest"));
    }

    public void ChangeMaterialWithAllChild(Transform tran, Material mat)
    {
        for (int ci = 0; ci < tran.childCount; ci++)
        {
            Button b = tran.GetChild(ci).GetComponent<Button>();
            if (b != null)
            {
                return;
            }

            //LeapEventTrigger et = tran.GetChild(ci).GetComponent<LeapEventTrigger>();
            //if (et != null)
            //{
            //    return;
            //}

            Image i = tran.GetChild(ci).GetComponent<Image>();
            Text t = tran.GetChild(ci).GetComponent<Text>();

            if (i != null)
            {
                i.material = mat;
            }

            if (t != null)
            {
                t.material = mat;
            }

            ChangeMaterialWithAllChild(tran.GetChild(ci), mat);
        }
    }


    public void AllChildOff(UITransform tran)
    {
        for (int ci = 0; ci < tran.transform.childCount; ci++)
        {
            tran.transform.GetChild(ci).gameObject.AddComponent<UITransform>();
            tran.transform.GetChild(ci).gameObject.SetActive(false);
        }
    }
    #endregion
    
    #region HMD Arrow
    public float time = 0;
    public GameObject mHmdArrow;

    public IEnumerator HMDArrowAngle(GameObject target, float pAngle)
    {
        bool loop = true;
        mHmdArrow = Get("HMD_Arrow");
      
        while (loop)
        {
            time += Time.deltaTime;

            if (target != null)
            {
                mHmdArrow.SetActive(true);
                mHmdArrow.transform.LookAt(target.transform);

                float angle = Vector3.Angle(playerCamera.transform.forward, (target.transform.position - playerCamera.transform.position).normalized);

                if (time > 1.7f)
                {
                    SoundMgr.Inst.Play(DefineEffect.좌우깜빡이는화살표소리);
                    time = 0;
                }

                if (angle <= pAngle)
                {
                    loop = false;
                    target = null;
                    SoundMgr.Inst.Stop(DefineEffect.좌우깜빡이는화살표소리);
                    mHmdArrow.SetActive(false);
                    time = 0;
                }
            }

            yield return null;
        }
    }

    #endregion

   

    #region 무빙
    #region HeadBob

    public HeadBob bob;
    public Coroutine footStep;

    private void PlayerHeadBobInit()
    {
        bob = Get("DeviceHeight").Get<HeadBob>();
        playerCamera = Get("VRCamera").Get<Camera>();
        bob.enabled = false;
    }

    IEnumerator PlayHeadBob(float duration)
    {
        TurnOnHeadBob();
        yield return new WaitForSeconds(duration);
        TurnOffHeadBob();
    }

    private void TurnOnHeadBob()
    {
        bob.GetComponent<AudioSource>().enabled = true;
        bob.GetComponent<AudioSource>().Play();
        bob.GetComponent<HeadBob>().enabled = true;
    }

    private void TurnOffHeadBob()
    {
        bob.GetComponent<AudioSource>().enabled = false;
        bob.GetComponent<AudioSource>().Stop();
        bob.GetComponent<HeadBob>().enabled = false;
    }

    #endregion

    #region 높이조정

    public void SetPlayerHeight(float height = 1.7f)
    {
        StartCoroutine(PlayerHeightSetting(height));
    }

    public IEnumerator PlayerHeightSetting(float height)
    {
        var pos = rootCollection.transform.localPosition;
        var ResultPos = new Vector3(pos.x, height, pos.z);
        var deltaPos = ResultPos - pos;

        float playTime = 0;
        float maxTime = 1.0f;

        while (playTime < maxTime)
        {
            playTime += UnityEngine.Time.deltaTime;
            var nowPos = rootCollection.transform.localPosition;
            nowPos += UnityEngine.Time.deltaTime * deltaPos / maxTime;
            rootCollection.transform.localPosition = nowPos;
            yield return null;
        }

        rootCollection.transform.localPosition = ResultPos;
    }

    #endregion

    #region 순간이동

    public void SetPosition(string positionName)
    {
        var VRCamera = Get("VRCamera");
        var Field = Get("VROrigin");
        var MovingTarget = GameState.Inst.NowLocation.Get(positionName);

        var intervalAngle = VRCamera.transform.eulerAngles.y - Field.transform.eulerAngles.y;

        var targetAngle = MovingTarget.transform.eulerAngles;
        targetAngle.y -= intervalAngle;

        Field.transform.eulerAngles = targetAngle;


        var interval = VRCamera.transform.position - Field.transform.position;
        interval.y = 0;

        Field.transform.position = MovingTarget.transform.position;
        Field.transform.position -= interval;
    }

    #endregion

    #region 걷고 순간이동

    //public void PlayMovingPoint(string name, string name2)
    //{
    //    StartCoroutine(move(playerTran.transform, positionDic[name].transform, positionDic[name2].transform));
    //}

    //public IEnumerator move(Transform mover, Transform target1, Transform target2)
    //{
    //    StartCoroutine(translates(mover, target1, target1.GetChild(0), 1.9f));
    //    yield return new WaitForSeconds(1.5f);
    //    StartCoroutine(playerUI.FadeOut(0.5f));

    //    yield return new WaitForSeconds(0.5f);

    //    StartCoroutine(translates(mover, target2, target2.GetChild(0), 1.9f));
    //    yield return new WaitForSeconds(0.5f);
    //    StartCoroutine(playerUI.FadeIn(0.5f));
    //}

    public IEnumerator translates(Transform target, Transform point1, Transform point2, float maxTime = 2.0f, bool isBob = true)
    {
        target.position = point1.position;
        target.rotation = point1.rotation;

        if (isBob)
        {
            if (footStep != null) StopCoroutine(footStep);
            footStep = StartCoroutine(PlayHeadBob(maxTime));
        }

        float playTime = 0;
        Vector3 deltaV = (point2.position - point1.position);
        while (playTime < maxTime)
        {
            playTime += Time.deltaTime;
            var pos = target.position;
            pos += deltaV * Time.deltaTime / maxTime;
            target.position = pos;
            yield return null;
        }

        target.position = point2.position;
        target.rotation = point2.rotation;
    }

    //public void PlayMovingPoint(string name, string name2, float duration0, float duration1)
    //{
    //    StartCoroutine(move(rootCollection.transform, positionDic[name].transform, positionDic[name2].transform, duration0, duration1));
    //}

    //public IEnumerator move(Transform mover, Transform target1, Transform target2, float duration0, float duration1)
    //{
    //    StartCoroutine(translates(mover, target1, target1.GetChild(0), duration0));
    //    yield return new WaitForSeconds(1.5f);
    //    StartCoroutine(playerUI.FadeOut(0.5f));

    //    yield return new WaitForSeconds(0.5f);

    //    StartCoroutine(translates(mover, target2, target2.GetChild(0), duration1));
    //    yield return new WaitForSeconds(0.5f);
    //    StartCoroutine(playerUI.FadeIn(0.5f));
    //}

    #endregion

    #region 경로 이동

    public void PlayPath(string name, float duration, bool isBob = true)
    {
        // duration *= 1.2f;

        pathDic[name].PlayPath(duration);

        if (isBob)
        {
            if (footStep != null) StopCoroutine(footStep);
            footStep = StartCoroutine(PlayHeadBob(duration));
        }
    }

    public void StopPath(string name)
    {
        pathDic[name].StopPath();
        StopCoroutine(footStep);
        TurnOffHeadBob();
    }

    public void PausePath(string name)
    {
        pathDic[name].PausePath();
        StopCoroutine(footStep);
        TurnOffHeadBob();
    }

    public void ResumePath(string name, float duration)
    {
        pathDic[name].ResumePath();

        if (footStep != null) StopCoroutine(footStep);
        footStep = StartCoroutine(PlayHeadBob(duration));
    }

    #endregion 
    #endregion

    #region 스크린 이펙트

    public PostProcessingBehaviour mPostProcess;
    public SENaturalBloomAndDirtyLens mDirtyLens;
    public CameraFilterPack_AAA_Blood_Plus blood;
    public BlinkEffect mBlinkEffect;
    public CameraFilterController controller;

    public void BlinkInit()
    {
        mBlinkEffect = Get("VRCamera").Get<BlinkEffect>();
        controller = Get("VRCamera").Get<CameraFilterController>();
    }

    #region Sream Fade
    //StaemVR Fade In & Out
    public void StartSteamFadeIn(float pMaxTime, System.Action pCall = null)
    {
        StartCoroutine(SteamVRFadeIn(pMaxTime, pCall));
    }

    public void StartSteamFadeOut(float pMaxTime, System.Action pCall = null)
    {
        StartCoroutine(SteamVRFadeOut(pMaxTime, pCall));
    }

    private IEnumerator SteamVRFadeIn(float pMaxTime, System.Action pCall = null)
    {
        //mSteamFade.OnStartFade(Color.black, pMaxTime, true);
        yield return new WaitForSeconds(pMaxTime + 2f);

        if (pCall != null)
        {
            pCall();
        }
    }

    private IEnumerator SteamVRFadeOut(float pMaxTime, System.Action pCall = null)
    {
        //mSteamFade.OnStartFade(Color.clear, pMaxTime, true);
        yield return new WaitForSeconds(pMaxTime + 1f);
        if (pCall != null)
        {
            pCall();
        }
    }
    #endregion

    #region Post Processing 비네트
    //PostVignetteFade In & Out
    public void StartPostVignetteFadeIn(float pMaxTime, System.Action pCall = null)
    {
        StartCoroutine(PostVignetteFadeIn(pMaxTime, pCall));
    }

    public void StartPostVignetteFadeOut(float pMaxTime, System.Action pCall = null)
    {
        StartCoroutine(PostVignetteFadeOut(pMaxTime, pCall));
    }

    private IEnumerator PostVignetteFadeIn(float pMaxTime, System.Action pCall = null)
    {
        for (float f = 0; f <= 1; f += Time.deltaTime / pMaxTime)
        {
            //구조체가 값타입이라서 통채로 바꿔야함.
            VignetteModel.Settings vignetteSetting = mPostProcess.profile.vignette.settings;
            vignetteSetting.intensity = Mathf.Lerp(0.6f, 0, f);
            mPostProcess.profile.vignette.settings = vignetteSetting;
            yield return null;
        }

        VignetteModel.Settings vignetteSetting2 = mPostProcess.profile.vignette.settings;
        vignetteSetting2.intensity = 0;
        mPostProcess.profile.vignette.settings = vignetteSetting2;

        if (pCall != null)
        {
            pCall();
        }
    }

    private IEnumerator PostVignetteFadeOut(float pMaxTime, System.Action pCall = null)
    {
        for (float f = 0; f <= 1; f += Time.deltaTime / pMaxTime)
        {
            VignetteModel.Settings vignetteSetting = mPostProcess.profile.vignette.settings;
            vignetteSetting.intensity = Mathf.Lerp(0, 0.7f, f);
            mPostProcess.profile.vignette.settings = vignetteSetting;

            yield return null;
        }

        VignetteModel.Settings vignetteSetting2 = mPostProcess.profile.vignette.settings;
        vignetteSetting2.intensity = 0.6f;
        mPostProcess.profile.vignette.settings = vignetteSetting2;

        if (pCall != null)
        {
            pCall();
        }
    }

    public void UpdateFadeRelease()
    {
        VignetteModel.Settings vignetteSetting = mPostProcess.profile.vignette.settings;
        vignetteSetting.intensity = 0.0f;
        mPostProcess.profile.vignette.settings = vignetteSetting;

        if (mDirtyLens != null)
            mDirtyLens.lensDirtIntensity = 0f;

        StopAllCoroutines();
    }

    private void ScreenEffectReset()
    {
        VignetteModel.Settings vignetteSetting = mPostProcess.profile.vignette.settings;
        vignetteSetting.intensity = 0.0f;
        mPostProcess.profile.vignette.settings = vignetteSetting;

        if (mDirtyLens != null)
            mDirtyLens.lensDirtIntensity = 0f;
    }

    bool loop = false;

    public void BlinkArrow(GameObject target, bool active)
    {
        if(active)
        {
            StartCoroutine(BlinkArrow(target));
        }
        else
        {
            Get("HMD_Arrow").SetActive(false);
            loop = false;
        }
    }

    IEnumerator BlinkArrow(GameObject target)
    {
        loop = true;
        mHmdArrow = Get("HMD_Arrow");
        //SoundMgr.Inst.Play(DefineSound.좌우깜빡이는화살표소리, 1.0f);
     
        mHmdArrow.SetActive(true);
        while (loop)
        {
            time += Time.deltaTime;
        
            if (target != null)
            {
                mHmdArrow.transform.LookAt(target.transform);
        
                if (time > 1.7f)
                {
                   // SoundMgr.Inst.Play(DefineSound.좌우깜빡이는화살표소리, 1.0f);
                    time = 0;
                }
            }
        
            yield return null;
        }

        Get("HMD_Arrow").SetActive(false);
        yield break;
    }

    #endregion

    #region EyeBlink

    //EyeBlink In, Out, In & Out
    public void EyeBlinkIn(System.Action pCall = null)
    {
        mBlinkEffect.FadeIn(pCall);
    }

    public void EyeBlinkOut(System.Action pCall = null)
    {
        mBlinkEffect.FadeOut(pCall);
    }

    public void EyeBlink(float pDelayTime, System.Action pCall = null)
    {
        StartCoroutine(Blink(pDelayTime, pCall));
    }

    public void EyeBlink(float pDelayTime, System.Action bpCall = null, System.Action pCall = null)
    {
        StartCoroutine(Blink(pDelayTime, bpCall, pCall));
    }

    private IEnumerator Blink(float pDelayTime, System.Action pCall = null)
    {
        mBlinkEffect.FadeIn();

        yield return new WaitForSeconds(pDelayTime);

        if (pCall != null)
        {
            pCall();
        }

        mBlinkEffect.FadeOut();
    }

    private IEnumerator Blink(float pDelayTime, System.Action bpCall = null, System.Action pCall = null)
    {
        if (bpCall != null)
        {
            bpCall();
        }

        mBlinkEffect.FadeIn();

        yield return new WaitForSeconds(pDelayTime);

        if (pCall != null)
        {
            pCall();
        }

        mBlinkEffect.FadeOut();
    }

    private IEnumerator Blink2(float pDelayTime, System.Action bpCall = null, System.Action pCall = null)
    {
        if (bpCall != null)
        {
            bpCall();
        }

        yield return new WaitForSeconds(pDelayTime);

        mBlinkEffect.FadeIn();

        yield return new WaitForSeconds(pDelayTime);

        if (pCall != null)
        {
            pCall();
        }

        mBlinkEffect.FadeOut();
    }
    #endregion

    #endregion

    
    public GameObject Get(string name)
    {
        if (rootCollection == null)
        {
            rootCollection = this.gameObject.GetParentRoot().AddComponent<CollectionableMonoBehaviour>();
          
        }

        return rootCollection.Get(name);
    }
}