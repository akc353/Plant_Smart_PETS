using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using cakeslice;
using Outline = cakeslice.Outline;

public class SimulationMgr : Singleton<SimulationMgr>
{
    Dictionary<string, GameObject> _EventObjectDic = new Dictionary<string, GameObject>();
    private string shadername = "Transparent/VertexLit with Z";
    private Coroutine fadeco;


    #region 공간싱크 조정 / 시작 위치 작성 및 바이브 싱크

    //public void SaveTransform_R()
    //{
    //    var ob = GameObject.Find("[ViveRoomScale]");

    //    Vector3 pos = ob.transform.localPosition;
    //    Vector3 rot = ob.transform.localEulerAngles;

    //    PlayerPrefs.SetFloat("px", pos.x);
    //    PlayerPrefs.SetFloat("py", pos.y);
    //    PlayerPrefs.SetFloat("pz", pos.z);

    //    PlayerPrefs.SetFloat("rx", rot.x);
    //    PlayerPrefs.SetFloat("ry", rot.y);
    //    PlayerPrefs.SetFloat("rz", rot.z);
    //}

    //public void LoadTransform_R()
    //{
    //    Vector3 pos = Vector3.zero;
    //    Vector3 rot = Vector3.zero;

    //    if (PlayerPrefs.HasKey("px"))
    //        pos.x = PlayerPrefs.GetFloat("px");
    //    if (PlayerPrefs.HasKey("py"))
    //        pos.y = PlayerPrefs.GetFloat("py");
    //    if (PlayerPrefs.HasKey("pz"))
    //        pos.z = PlayerPrefs.GetFloat("pz");

    //    if (PlayerPrefs.HasKey("rx"))
    //        rot.x = PlayerPrefs.GetFloat("rx");
    //    if (PlayerPrefs.HasKey("ry"))
    //        rot.y = PlayerPrefs.GetFloat("ry");
    //    if (PlayerPrefs.HasKey("rz"))
    //        rot.z = PlayerPrefs.GetFloat("rz");

    //    var ob = GameObject.Find("[ViveRoomScale]");
    //    ob.transform.localPosition = pos;
    //    ob.transform.localRotation = Quaternion.Euler(rot);
    //}

    //private void MakePerfect()
    //{
    //    SetFirstRotation();
    //    SetFirstPosition();
    //    //SetHeight(1.45f);

    //    SaveTransform_R();
    //}

    //public void SetFirstRotation()
    //{
    //    Transform tran = GameObject.Find("[ViveRoomScale]").transform;
    //    tran.localRotation = Quaternion.identity;

    //    string axis = string.Empty;
    //    int reverse = 1;
    //    Vector3 direction; // = GetDirection(PlayerCameraObj.transform.forward, out axis, out reverse);

    //    if (axis == "X")
    //    {
    //        if (reverse == -1)
    //        {
    //            Debug.Log("X 보정");
    //            tran.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
    //        }
    //    }

    //    if (axis == "Y")
    //    {
    //        Debug.Log("Y 보정");
    //    }

    //    if (axis == "Z")
    //    {
    //        Debug.Log("Z 보정 : " + reverse);
    //        tran.rotation = Quaternion.Euler(new Vector3(0, 90 * reverse, 0));
    //    }
    //}

    //public void SetFirstPosition()
    //{
    //    var field = GameObject.Find("[ViveRoomScale]");
    //    field.transform.localPosition = new Vector3(-1.5f, 0f, 1f);
    //    //field.transform.localRotation = Quaternion.identity;
    //    field.transform.localScale = Vector3.one;

    //    var startPosition = GameObject.Find("[StartPosition]");
    //    Vector3 startpos = startPosition.transform.position;
    //    startpos.y = 0;

    //    Vector3 vrcamerapos = PlayerCameraObj.transform.position;
    //    vrcamerapos.y = 0;

    //    Vector3 distance = vrcamerapos - startpos;
    //    // Debug.Log("이만큼 차이남 : " + distance);
    //    field.transform.localPosition = field.transform.localPosition += (distance * -1);
    //}

    //public void SetHeight(float HardwareHeight)
    //{
    //    float HeightOffset = 1.45f - HardwareHeight;

    //    var field = GameObject.Find("[ViveRoomScale]");
    //    Vector3 pos = field.transform.localPosition;
    //    pos.y += HeightOffset;
    //    field.transform.localPosition = pos;
    //}

    //public Vector3 GetDirection(Vector3 VRCameraForward, out string axis, out int reverse)
    //{
    //    Vector3 direction = Vector3.zero;

    //    if (Mathf.Abs(VRCameraForward.x) > Mathf.Abs(VRCameraForward.y))
    //    {
    //        if (Mathf.Abs(VRCameraForward.x) > Mathf.Abs(VRCameraForward.z))
    //        {
    //            direction.x = 1.0f;

    //            axis = "X";
    //            if (VRCameraForward.x > 0)
    //                reverse = 1;
    //            else
    //                reverse = -1;

    //            return direction;
    //        }
    //    }
    //    else
    //    {
    //        if (Mathf.Abs(VRCameraForward.y) > Mathf.Abs(VRCameraForward.z))
    //        {
    //            direction.y = 1.0f;
    //            axis = "Y";
    //            if (VRCameraForward.y > 0)
    //                reverse = 1;
    //            else
    //                reverse = -1;

    //            return direction;
    //        }
    //    }

    //    direction.z = 1.0f;

    //    axis = "Z";
    //    if (VRCameraForward.z > 0)
    //        reverse = 1;
    //    else
    //        reverse = -1;

    //    return direction;
    //}

    #endregion 공간싱크 조정



    public IEnumerator translate(Transform rig, Vector3 resultPos, float value)
    {
        if (value == 0)
        {
            rig.position = resultPos;
            yield break;
        }

        float nowTime = 0;
        Vector3 deltaV = (resultPos - rig.position);
        deltaV.Normalize();
        deltaV *= value;

        float preDistance = Mathf.Abs(resultPos.y - rig.transform.position.y);

        while (Mathf.Abs(resultPos.y - rig.transform.position.y) <= preDistance)
        {
            preDistance = Mathf.Abs(resultPos.y - rig.transform.position.y);

            nowTime += Time.deltaTime;
            var pos = rig.position;
            pos += new Vector3(0, deltaV.y, 0) * Time.deltaTime;
            rig.position = pos;

            yield return null;
        }

        preDistance = Mathf.Abs((resultPos - rig.transform.position).magnitude);
        deltaV.y = 0;

        while (Mathf.Abs((resultPos - rig.transform.position).magnitude) <= preDistance)
        {
            preDistance = Mathf.Abs((resultPos - rig.transform.position).magnitude);

            nowTime += Time.deltaTime;
            var pos = rig.position;
            pos += deltaV * Time.deltaTime;
            rig.position = pos;

            yield return null;
        }
    }

    const float GapY = 11.5f;

    [Range(0f, 100f)]
    public float ObjHeight; //머리 부딪히는 높이 7.5f

    //isPickUp : Goal을 향해 인양하러 가는 중, isInit : Goal의 위치로 바로 이동, rig : 움직이는 대상, Goal : 목표 지점, 
    //XTime : 수평으로 움직일 총 시간, YTime : 수직으로 움직일 총시간, ObjHeight : 인양할 오브젝트 높이(머리 부딪히는 높이 : 7.5f)
    public IEnumerator translate_MoveCrane(bool isPickUp, bool isInit, Transform rig, Transform Goal , 
                                            int XTime = 2, int YTime = 2, float? ObjHeight = null, float RealXTime = -1, float RealYTime = -1)
    {
        Vector3 GoalPos = Goal.position;

        if (RealXTime == -1) RealXTime = XTime;
        if (RealYTime == -1) RealYTime = YTime;
        if (ObjHeight.Equals(null)) ObjHeight = this.ObjHeight;

        if (isPickUp)
        {
            var pos = GoalPos;
            pos.y = GapY + (float)ObjHeight;

            GoalPos = pos;
        }

        if (isInit)
        {
            var pos = GoalPos;
            pos.y = GapY + (float)ObjHeight;
            GoalPos = pos;
            rig.position = new Vector3(GoalPos.x, GoalPos.y, rig.position.z);

            yield break;
        }
        
        float deltaX = GoalPos.x - rig.position.x;
        float deltaY = GoalPos.y - rig.position.y;

        if (!isPickUp)
            deltaY += GapY;

        float XperTime = deltaX / (XTime * 100);
        float YperTime = deltaY / (YTime * 100);

        for (int i = 0; i < RealYTime * 100; i++)
        {
            var pos = rig.position;
            pos.y += YperTime;

            rig.position = pos;

            yield return new WaitForSeconds(0.01f);
        }

        rig.position = new Vector3(rig.position.x, GoalPos.y, rig.position.z);

        for (int i = 0; i < RealXTime * 100; i++)
        {
            var pos = rig.position;
            pos.x += XperTime;

            rig.position = pos;

            yield return new WaitForSeconds(0.01f);
        }

        rig.position = new Vector3(GoalPos.x, rig.position.y, rig.position.z);
    }


    //public IEnumerator translate_MoveCraneZ(bool isPickUp, bool isInit, Transform rig, Transform Goal,
    //                                        int ZTime = 2, int YTime = 2, float? ObjHeight = null, float RealZTime = -1, float RealYTime = -1)
    //{
    //    Vector3 GoalPos = Goal.position;
    //
    //    if (RealZTime == -1) RealZTime = ZTime;
    //    if (RealYTime == -1) RealYTime = YTime;
    //    if (ObjHeight.Equals(null)) ObjHeight = this.ObjHeight;
    //
    //    if (isPickUp)
    //    {
    //        var pos = GoalPos;
    //        pos.y = GapY + (float)ObjHeight;
    //
    //        GoalPos = pos;
    //    }
    //
    //    if (isInit)
    //    {
    //        var pos = GoalPos;
    //        pos.y = GapY + (float)ObjHeight;
    //        GoalPos = pos;
    //        rig.position = new Vector3(rig.position.x, GoalPos.y, GoalPos.z);
    //
    //        yield break;
    //    }
    //
    //    float deltaZ = GoalPos.z - rig.position.z;
    //    float deltaY = GoalPos.y - rig.position.y;
    //
    //    if (!isPickUp)
    //        deltaY += GapY;
    //
    //    float ZperTime = deltaZ / (ZTime * 100);
    //    float YperTime = deltaY / (YTime * 100);
    //
    //    for (int i = 0; i < RealYTime * 100; i++)
    //    {
    //        var pos = rig.position;
    //        pos.y += YperTime;
    //
    //        rig.position = pos;
    //
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    rig.position = new Vector3(rig.position.x, GoalPos.y, rig.position.z);
    //
    //    for (int i = 0; i < RealZTime * 100; i++)
    //    {
    //        var pos = rig.position;
    //        pos.z += ZperTime;
    //
    //        rig.position = pos;
    //
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    rig.position = new Vector3(rig.position.x, rig.position.y, GoalPos.z);
    //}

    //public void Awake()
    //{
    //    InitOutlineObject();
    //}

    //public void TriggerOn(Actor actor)
    //{
    //    actor.TargetObject.GetComponent<BoxCollider>().isTrigger = true;
    //}

    //public void TriggerOff(Actor actor)
    //{
    //    actor.TargetObject.GetComponent<BoxCollider>().isTrigger = false;
    //}

    //public void Drop(Actor actor)
    //{
    //    actor.TargetObject.GetComponent<Rigidbody>().useGravity = true;
    //}

    //public void Visible(Actor actor)
    //{
    //    actor.TargetObject.GetComponent<MeshRenderer>().enabled = true;
    //}

    //public void Invisible(Actor actor)
    //{
    //    actor.TargetObject.GetComponent<MeshRenderer>().enabled = false;
    //}

    public void ImportResetObject(GameObject go)
    {
        //ResetObjectDic.Add();
    }

    //public Action<Actor> FindAction(string ActionName)
    //{
    //    return (Action<Actor>)Delegate.CreateDelegate(typeof(Action<Actor>), this, this.GetType().GetMethod(ActionName));
    //}

   
    private GameObject eye;


    

    //public void 페이드인아웃RawImage(RawImage image, float time, float time2, float time3)
    //{
    //    StartCoroutine(RawImageFadeInOut(image, time, time2, time3));
    //}

    //public IEnumerator RawImageFadeInOut(RawImage image, float time, float time2, float time3)
    //{
    //    StartCoroutine(Co_FadeInRawImage(image, time, 1));
    //    yield return new WaitForSeconds(time2);
    //    StartCoroutine(Co_FadeOutRawImage(image, time3, 1));
    //}


    public IEnumerator Co_FadeImageFromTo(Image image, float time, float from, float to)
    {
        image.gameObject.SetActive(true);

        Image i = image;
        Color c = i.color;
        float timer = 0;

        do
        {
            timer += Time.deltaTime;
            c.a = 1 - Mathf.Clamp(timer / time, from, to);
            i.color = c;
            yield return null;

        } while (timer < time);
    }

    public void UIButtonEvent(UITransform uiButton, System.Action action)
    {
        var t = uiButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(x => action());
        t.triggers.Add(entry);
    }

    public void UIButtonEvent(GameObject uiButton, System.Action action)
    {
        var t = uiButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(x => action());
        t.triggers.Add(entry);
    }

 

  

    public IEnumerator translates(Transform rig)
    {
        float nowTime = 0;
        Vector3 deltaV = new Vector3(0.2f, 0, 0);

        while (nowTime < 1.0f)
        {
            nowTime += Time.deltaTime;
            var pos = rig.position;
            pos += deltaV * Time.deltaTime;
            rig.position = pos;
            yield return null;
        }
    }

    public IEnumerator translates(Transform rig, Transform target)
    {
        float nowTime = 0;
        Vector3 deltaV = (target.position - rig.position);
        deltaV.Normalize();
        deltaV *= 0.1f;

        while (deltaV.magnitude < 0.1f)
        {
            nowTime += Time.deltaTime;
            var pos = rig.position;
            pos += deltaV * Time.deltaTime;
            rig.position = pos;
            yield return null;
        }
    }

    public IEnumerator translates(Transform rig, Vector3 resultPos)
    {
        float nowTime = 0;
        Vector3 deltaV = (resultPos - rig.position);
        deltaV.Normalize();
        deltaV *= 0.1f;

        while ((resultPos - rig.transform.position).magnitude > 0.1f)
        {
            nowTime += Time.deltaTime;
            var pos = rig.position;
            pos += deltaV * Time.deltaTime;
            rig.position = pos;
            yield return null;
        }
    }

    public IEnumerator translates(Transform basic, Transform mover, Vector3 resultPos)
    {
        float nowTime = 0;
        var realResultPos = resultPos;
        realResultPos.z = basic.position.z;
        Vector3 deltaV = (realResultPos - basic.position);
        deltaV.Normalize();
        deltaV *= 0.1f;

        while ((realResultPos - basic.transform.position).magnitude > 0.01f)
        {
            nowTime += Time.deltaTime;
            var pos = mover.position;
            pos += deltaV * Time.deltaTime;
            //pos.z = mover.position.z;
            mover.position = pos;
            yield return null;
        }
    }

    protected void SetCollider(GameObject obj, bool enable)
    {
        obj.GetComponent<Collider>().enabled = enable;
    }

    float nowTime = 0;
    IEnumerator FrontAnimation(GameObject ob, float time)
    {
        while (time < nowTime)
        {
            nowTime += Time.deltaTime;

            Vector3 pos = ob.transform.position;
            pos.x += nowTime;
            ob.transform.position = pos;

            Debug.Log(time + " // " + nowTime + " // " + pos);

            yield return null;
        }

        nowTime = 0;
    }

    public IEnumerator FadeInOutGameObject(GameObject ob, float time1, float time2, float time3, Camera c)
    {
        if (c != null)
        {
            c.enabled = true;
            c.gameObject.SetActive(true);
        }

        StartCoroutine(FadeInObject(ob, time1, null));
        yield return new WaitForSeconds(time2);
        StartCoroutine(FadeOutObject(ob, time3, true, null));
        yield return new WaitForSeconds(time3);

        if (c != null)
        {
            c.gameObject.SetActive(false);
            c.enabled = false;
        }
    }

    private IEnumerator FadeInObject(GameObject pGo, float maxTime, System.Action action)
    {
        pGo.SetActive(true);
        Renderer[] renderer = pGo.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderer.Length; ++i)
        {
            //메테리얼이 여러개일때면
            if (renderer[i].materials.Length > 1)
            {
                Debug.Log("여러개인 메테리얼" + pGo.name);
                //Material[] mr1 = mr[i].materials;
                for (int j = 0; j < renderer[i].materials.Length; ++j)
                {
                    renderer[i].materials[j].shader = Shader.Find(shadername);
                }
            }
            else
            {
                renderer[i].material.shader = Shader.Find(shadername);
            }
        }

        Color[] originColor;
        //Renderer[] renderer = pGo.GetComponentsInChildren<Renderer>();
        originColor = new Color[renderer.Length];

        for (int i = 0; i < renderer.Length; ++i)
        {
            originColor[i] = renderer[i].material.color;
        }

        for (float t = 0; t <= maxTime; t += Time.deltaTime)
        {
            for (int i = 0; i < renderer.Length; ++i)
            {
                Color destColor = originColor[i];
                destColor.a = 1f;

                originColor[i].a = 0f;

                Material[] mats = renderer[i].materials;

                for (int mi = 0; mi < mats.Length; ++mi)
                    mats[mi].color = Color.Lerp(originColor[i], destColor, t / maxTime);
            }
            yield return null;
        }

        for (int i = 0; i < renderer.Length; ++i)
        {
            //메테리얼이 여러개일때면
            if (renderer[i].materials.Length > 1)
            {
                Debug.Log("여러개인 메테리얼" + pGo.name);
                //Material[] mr1 = mr[i].materials;
                for (int j = 0; j < renderer[i].materials.Length; ++j)
                {
                    // renderer[i].materials[j].shader = Shader.Find("Standard");
                }
            }
            else
            {
                // renderer[i].material.shader = Shader.Find("Standard");
            }
        }

        if (action != null)
        {
            action();
        }
    }

    public void FadeOutGameObject(GameObject pGo, float maxTime, bool afterOff, System.Action action)
    {
        StartCoroutine(FadeOutObject(pGo, maxTime, afterOff, action));
    }

    private IEnumerator FadeOutObject(GameObject pGo, float maxTime, bool afterOff, System.Action action)
    {
        //Renderer[] mr = pGo.GetComponentsInChildren<Renderer>();
        pGo.SetActive(true);
        Renderer[] renderer = pGo.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderer.Length; ++i)
        {
            //메테리얼이 여러개일때면
            if (renderer[i].materials.Length > 1)
            {
                Debug.Log("여러개인 메테리얼" + pGo.name);
                //Material[] mr1 = mr[i].materials;
                for (int j = 0; j < renderer[i].materials.Length; ++j)
                {
                    renderer[i].materials[j].shader = Shader.Find(shadername);
                }
            }
            else
            {
                renderer[i].material.shader = Shader.Find(shadername);
            }
        }

        //Color[] originColor;
        //originColor = new Color[renderer.Length];

        //int index = 0;
        //for (int i = 0; i < renderer.Length; ++i)
        //{
        //    if(renderer[i].materials.Length > 1)
        //    {
        //        for (int j = 0; j < renderer[i].materials.Length; ++j)
        //            originColor[index] = renderer[i].materials[j].color;
        //    }
        //    else
        //    {
        //        originColor[index] = renderer[i].material.color;
        //    }
        //    index++;
        //}

        for (float t = 0; t <= maxTime; t += Time.deltaTime)
        {
            for (int i = 0; i < renderer.Length; ++i)
            {
                //Color destColor = originColor[i];
                //Color destColor = renderer[i].material.color

                //destColor.a = 0f;

                Material[] mats = renderer[i].materials;

                for (int mi = 0; mi < mats.Length; ++mi)
                {
                    Color destColor = renderer[i].materials[mi].color;
                    destColor.a = 0f;
                    mats[mi].color = Color.Lerp(renderer[i].materials[mi].color, destColor, t / maxTime);
                }

            }
            yield return null;
        }

        if (afterOff)
        {
            pGo.SetActive(false);
        }

        if (action != null)
        {
            action();
        }

        pGo.SetActive(false);
    }
}
