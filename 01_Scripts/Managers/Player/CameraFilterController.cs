using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FilterSort
{
    NONE,
    ROBOTARM,
    PRINTER,
    FORKLIFT,
    CUTTER
}

public class CameraFilterController : MonoBehaviour
{
    delegate void filterTo();
    filterTo _filterTo;
    float speed = 1.0f;

    public class FilteringElements
    {
        public float COLOR_RED;
        public float COLOR_BRIGHTNESS;
        public float DRAWING_INTENSITY;
        public float BLUR_SIZE;
        public float BLOOD_LIGHTREFLECT;

        public FilteringElements
            (
            float color_red = 0.0f, float color_brightness = 0.0f, float drawing_intensity = 0.0f,
            float blur_size = 0.5f, float blood_lightreflect = 0.0f)
        {
            COLOR_RED = color_red;
            COLOR_BRIGHTNESS = color_brightness;
            DRAWING_INTENSITY = drawing_intensity;
            BLUR_SIZE = blur_size;
            BLOOD_LIGHTREFLECT = blood_lightreflect;
        }
    }

    #region 필터 요소 저장
    FilteringElements NONE_FILTER;
    FilteringElements ROBOTARM_FILTER;
    FilteringElements PRINTER_FILTER;
    FilteringElements FORKLIFT_FILTER;
    FilteringElements CUTTER_FILTER;
    #endregion

    #region 현재 필터
    FilteringElements CURRENT_FILTER;
    #endregion

    #region 카메라의 필터 컴포넌트들
    [SerializeField]
    CameraFilterPack_Colors_Adjust_ColorRGB m_Filter_COLOR;
    [SerializeField]
    CameraFilterPack_Drawing_Manga_FlashWhite m_Filter_DRAWING;
    [SerializeField]
    CameraFilterPack_Blur_Focus m_Filter_BLUR;
    [SerializeField]
    CameraFilterPack_AAA_Blood_Plus m_Filter_BLOOD;

    //CameraFilterPack_Blend2Camera_Blend m_BlendCamera;
    #endregion

    bool isProcessing;

    //void setBlendCamera()
    //{
    //    //m_BlendCamera = GetComponent<CameraFilterPack_Blend2Camera_Blend>();
    //    //if (m_BlendCamera == null)
    //    //   m_BlendCamera = gameObject.AddComponent <CameraFilterPack_Blend2Camera_Blend>();
    //    
    //    //if(m_BlendCamera.Camera2 == null)
    //    //{
    //        GameObject Camera2 = GameObject.Find("Camera2");
    //        if(Camera2 == null)
    //        {
    //            Camera2 = new GameObject("Camera2");
    //    
    //            Camera2.AddComponent<Camera>();
    //        }
    //    
    //        m_BlendCamera.Camera2 = Camera2.Get<Camera>();
    //    
    //    
    //        Camera2.Get<Camera>().targetDisplay = 3;
    //        Camera2.Get<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
    //        Camera2.Get<Camera>().renderingPath = RenderingPath.DeferredShading;
    //        Camera2.Get<Camera>().fieldOfView = 110f;
    //        Camera2.Get<Camera>().allowMSAA = false;
    //        Camera2.Get<Camera>().depth = -1;
    //    //}
    //}

   

    //public void StartBlend(float time)
    //{
    //    StartCoroutine(co_Blend(time));
    //}
    //
    //IEnumerator co_Blend(float time)
    //{
    //    float playTime = 0;
    //    m_BlendCamera.BlendFX = 1f;
    //    m_BlendCamera.enabled = true;
    //
    //    while (time >= playTime)
    //    {
    //        playTime += Time.deltaTime;
    //
    //        m_BlendCamera.BlendFX = 1f - (playTime / time);
    //        yield return null;
    //    }
    //
    //    m_BlendCamera.BlendFX = 0f;
    //    m_BlendCamera.enabled = false;
    //}

    private void Awake()
    {
        NONE_FILTER = new FilteringElements();
        ROBOTARM_FILTER = new FilteringElements(0.2f, -0.1f, 0.0f, 0.5f, 0.654f);
        PRINTER_FILTER = new FilteringElements(0.0f, -0.1f, 0.2f, 8.0f, 0.0f);
        FORKLIFT_FILTER = new FilteringElements(0.05f, 0.0f, 0.2f, 0.5f, 0.0f);
        CUTTER_FILTER = new FilteringElements(0.1f, 0.0f, 0.0f, 5.0f, 0.0f);

        m_Filter_COLOR = GetComponent<CameraFilterPack_Colors_Adjust_ColorRGB>();
        m_Filter_DRAWING = GetComponent<CameraFilterPack_Drawing_Manga_FlashWhite>();
        m_Filter_BLUR = GetComponent<CameraFilterPack_Blur_Focus>();
        m_Filter_BLOOD = GetComponent<CameraFilterPack_AAA_Blood_Plus>();
        //setBlendCamera();


        //m_Filter_COLOR.enabled = false;
        //m_Filter_DRAWING.enabled = false;
        //m_Filter_BLUR.enabled = false;
        //m_Filter_BLOOD.enabled = false;

        CURRENT_FILTER = NONE_FILTER;
        _filterTo = NONE;
    }

    private void FixedUpdate()
    {
        _filterTo();
    }

    private void NONE()
    {

    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        SetFilter(FilterSort.CUTTER);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        SetFilter(FilterSort.FORKLIFT);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        SetFilter(FilterSort.NONE);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha4))
    //    {
    //        SetFilter(FilterSort.PRINTER);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        SetFilter(FilterSort.ROBOTARM);
    //    }
    //}

    private void StartAndStopLerp()
    {
        StartCoroutine(OneWaitOff());
    }

    IEnumerator OneWaitOff()
    {
        if (!isProcessing)
        {
            m_Filter_DRAWING.enabled = true;
            m_Filter_COLOR.enabled = true;
            m_Filter_BLUR.enabled = true;
            m_Filter_BLOOD.enabled = true;

            isProcessing = true;
            _filterTo += LerpFilter;
            yield return new WaitForSeconds(5.0f);
            _filterTo -= LerpFilter;
            isProcessing = false;

            m_Filter_DRAWING.enabled = false;
            m_Filter_COLOR.enabled = false;
            m_Filter_BLUR.enabled = false;
            m_Filter_BLOOD.enabled = false;
        }
    }

    float _speed;
    private void LerpFilter()
    {
        _speed = speed * Time.deltaTime;
        m_Filter_COLOR.Red = Mathf.Lerp(m_Filter_COLOR.Red, CURRENT_FILTER.COLOR_RED, _speed);
        m_Filter_COLOR.Brightness = Mathf.Lerp(m_Filter_COLOR.Brightness, CURRENT_FILTER.COLOR_BRIGHTNESS, _speed);
        m_Filter_DRAWING.Intensity = Mathf.Lerp(m_Filter_DRAWING.Intensity, CURRENT_FILTER.DRAWING_INTENSITY, _speed);
        m_Filter_BLUR._Size = Mathf.Lerp(m_Filter_BLUR._Size, CURRENT_FILTER.BLUR_SIZE, _speed);
        m_Filter_BLOOD.LightReflect = Mathf.Lerp(m_Filter_BLOOD.LightReflect, CURRENT_FILTER.BLOOD_LIGHTREFLECT, _speed);
        m_Filter_BLOOD.Blood_12 = Mathf.Lerp(0, 1, _speed);
        m_Filter_BLOOD.Blood_11 = Mathf.Lerp(0, 1, _speed);
        m_Filter_BLOOD.Blood_10 = Mathf.Lerp(0, 1, _speed);
        m_Filter_BLOOD.Blood_9 = Mathf.Lerp(0, 1, _speed);
        m_Filter_BLOOD.Blood_8 = Mathf.Lerp(0, 1, _speed);
    }

    public void ResetFilter()
    {
        isProcessing = false;

        StopCoroutine(OneWaitOff());

        m_Filter_COLOR.Red = 0.0f;
        m_Filter_COLOR.Brightness = 0.0f;
        m_Filter_DRAWING.Speed = 0;
        m_Filter_DRAWING.Intensity= 0.0f;
        m_Filter_BLUR._Size = 0.14f;
        m_Filter_BLOOD.LightReflect = 0.0f;
    }

    public void SetFilter(FilterSort filterSort)
    {
        switch (filterSort)
        {
            case FilterSort.NONE: CURRENT_FILTER = NONE_FILTER;
                m_Filter_DRAWING.Speed = 0;
                break;
            case FilterSort.CUTTER: CURRENT_FILTER = CUTTER_FILTER;
                m_Filter_DRAWING.Speed = 0;
                break;
            case FilterSort.PRINTER: CURRENT_FILTER = PRINTER_FILTER;
                m_Filter_DRAWING.Speed = 1;
                break;
            case FilterSort.FORKLIFT: CURRENT_FILTER = FORKLIFT_FILTER;
                m_Filter_DRAWING.Speed = 16;
                break;
            case FilterSort.ROBOTARM: CURRENT_FILTER = ROBOTARM_FILTER;
                m_Filter_DRAWING.Speed = 0;
                break;
        }

        StartAndStopLerp();
    }
}