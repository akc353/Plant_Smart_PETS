using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct sUI_Data
{
    public string FilePath;         //경로
    public string Name;            //이미지 파일 이름
 
    public sUI_Data(string pFilePath, string pName)
    {
        FilePath = pFilePath;
        Name = pName;

    }
}

public class UIMgr : Singleton<UIMgr>
{
    public CollectionableMonoBehaviour rootCollection;

    //상단,하단 path,name 데이터 리스트
    public List<sUI_Data> mUPUIDataList = new List<sUI_Data>();
    public List<sUI_Data> mDownUIDataList = new List<sUI_Data>();

    private List<GameObject> mUpUIList = new List<GameObject>();  //상단탭 오브젝트 리스트
    private List<GameObject> mDownUIList = new List<GameObject>(); //하단탭 오브젝트 리스트
    public Slider mHUDSlider;
    public GameObject mHUD_RingGauge;
    public Image mHUD_RingTop;


    public GameObject Get(string name)
    {
        if (rootCollection == null)
        {
            rootCollection = PlayerMgr.Inst.rootCollection;
        }

        return rootCollection.Get(name);
    }


    //Fade Img
    // 페이드인아웃
    public void FadeINOutUIImage(string imageName, float inTime = 0.5f, float Time = 2.5f, float outTime = 0.5f)
    {
        StartCoroutine(ImageFadeInOut(Get(imageName).Get<Image>(), inTime, Time, outTime));
    }

 
    public void FadeInUIImage(string imageName, float time = 0.5f)
    {
        StartCoroutine(Co_FadeInImage(Get(imageName).Get<Image>(), time));
    }

    public void FadeOutUIImage(string imageName, float time = 0.3f)
    {
        StartCoroutine(Co_FadeOutImage(Get(imageName).Get<Image>(), time));
    }


    //Ring Gauge
    public void SetRingGauge(bool pSet)
    {
        mHUD_RingTop.fillAmount = 0;
        if (pSet)
            mHUD_RingGauge.SetActive(pSet);
        else
            mHUD_RingGauge.SetActive(!pSet);
        
    }
    
    public void RingUIFill(float value)
    {
        mHUD_RingTop.fillAmount = value;
    }


    #region 상단탭, 하단탭 데이터 추가, 삭제
    //상단탭 path,name 데이터 추가
    public void InitUpUIDataList(List<string> pUINameList)
    {
        this.mUPUIDataList.Clear();
        for (int i = 0; i < pUINameList.Count; i++)
        {
            mUPUIDataList.Add(new sUI_Data("Image/UI/상단탭/", pUINameList[i]));
        }

        this.mUpUIList.Clear();
        for (int i = 1; i < GameObject.Find("상단메시지패널").transform.childCount; i++)
        {
            this.mUpUIList.Add(GameObject.Find("상단메시지패널").transform.GetChild(i).gameObject);
        }
    }

    //하단탭 path,name 데이터 추가
    public void InitDownUIDataList(List<string> pDownUINameList)
    {
        this.mDownUIDataList.Clear();
        for (int i = 0; i < pDownUINameList.Count; i++)
        {
            mDownUIDataList.Add(new sUI_Data("Image/UI/하단탭/", pDownUINameList[i]));
        }

        this.mDownUIList.Clear();
        for (int i = 1; i < GameObject.Find("하단메시지패널").transform.childCount; i++)
        {
            this.mDownUIList.Add(GameObject.Find("하단메시지패널").transform.GetChild(i).gameObject);
        }
    }


    //상단탭 오브젝트 리스트들 싹 한번 날림.
    public void DeleteAllUpUIImg()
    {
        for (int i = 0; i < mUpUIList.Count; i++)
        {
            DestroyImmediate(mUpUIList[i].gameObject);
        }
        mUpUIList.Clear();
        this.DeleteCloneResourceMgr();
    }


    //하단탭 오브젝트 리스트들 싹 한번 날림.
    public void DeleteAllDownUIImg()
    {
        for (int i = 0; i < mDownUIList.Count; i++)
        {
            DestroyImmediate(mDownUIList[i].gameObject);
        }
        mDownUIList.Clear();
        this.DeleteCloneResourceMgr();
    }

    //생성 되서 남아있는 리소스매니저 더미들 삭제.
    public void DeleteCloneResourceMgr()
    {
        ResourceMgr[] cloneResourceMgr = FindObjectsOfType<ResourceMgr>();
        for (int i = 0; i < cloneResourceMgr.Length; i++)
        {
            DestroyImmediate(cloneResourceMgr[i].gameObject);
        }
    }

    #endregion


    #region 이미지 fade기능들
    private IEnumerator ImageFadeInOut(Image image, float inTime, float time, float OutTime)
    {
        StartCoroutine(Co_FadeInImage(image, inTime));
        yield return new WaitForSeconds(inTime + time);
        StartCoroutine(Co_FadeOutImage(image, OutTime));
    }

    private IEnumerator Co_FadeInImage(Image image, float time)
    {
        image.gameObject.SetActive(true);

        Image i = image;
        Color c = i.color;
        float timer = 0;

        do
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp(timer / time, 0f, 1f);
            i.color = c;
            yield return null;

        } while (timer < time);
    }

  
    private IEnumerator Co_FadeOutImage(Image image, float time)
    {
        image.gameObject.SetActive(true);

        Image i = image;
        Color c = i.color;
        float timer = 0;

        do
        {
            timer += Time.deltaTime;
            c.a = 1 - Mathf.Clamp(timer / time, 0f, 1f);
            i.color = c;
            yield return null;

        } while (timer < time);

        image.gameObject.SetActive(false);
    }


    #endregion



}

