using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using SimpleJSON;


//추가 수정사항, 요구사항 받음. - 이가연 
//공통적이게 중복인거 따로 빼야하는데 어느정도 나오면  작업 예정.
//사운드 풀링은 여기서 돌리지 않습니다. 여기는 오로지 기능만 있을겁니다.

public enum eSOUNDTYPE
{
    NONE = 0,
    TTS,
    EFFECT
}

public class SoundMgr : Singleton<SoundMgr>
{
    private List<SoundSource> mSoundList = new List<SoundSource>();   //플레이한 사운드 넣어줄 리스트
    private string mSoundListFilePath = "Sound/JSON/TTSList";            //사운드 목록들 적혀있는 리스트 파일 경로, 리스ㅇ트
    private string mEffectListFilePath= "Sound/JSON/EffectList";


    private SoundPooling mSoundPooling;
    private SoundClipStorage mSoundClipStorage;
    private SoundClipStorage mEffectClipStorage;

    //bgm사용 할시, 안 할시 나중에 작업추가. 나중에 수정.
    private GameObject mBgmSound;
    public AudioSource mAudioBGM;



    public void InitSoundMgr()
    {
        mSoundPooling = new SoundPooling();

        mSoundClipStorage = new SoundClipStorage(mSoundListFilePath, mEffectListFilePath);
        CreateBgm();
        BgmSound(true, 0.06f);
    }


    #region BGM Function

    public void CreateBgm()
    {
        //bgm은 하나 생성해서 관리. 풀링x. TTS나 이펙트 (fx)만 돌릴것임.
        mBgmSound = new GameObject("BGM");
        mBgmSound.transform.SetParent(this.transform);
        mAudioBGM = mBgmSound.AddComponent<AudioSource>();
    }

    public void BgmSound(bool pPlay = true, float pVolume = 0.06f)
    {
        if (mAudioBGM != null)
        {
           mAudioBGM.clip = mSoundClipStorage.GetClip(DefineEffect.LGD협착기본환경음); //사용할 클립으로 알아서 바꿔주세요
           if (pPlay) mAudioBGM.Play();
           else mAudioBGM.Stop();
            
            mAudioBGM.volume = pVolume;
            mAudioBGM.loop = true;
        }
    }

    #endregion BGM Function

    //사운드 그냥 플레이
    public void Play(string pClipKey, float pVoloume=1f)
    {
        SoundSource soundSource = null;
        if (mSoundPooling.HasSoundSource())
        {
            soundSource = mSoundPooling.GetSoundSource();
            soundSource.gameObject.SetActive(true);
        }
        else
        {
            soundSource = mSoundPooling.CreateSoundSource(this.transform);
        }

        soundSource.mSoundSource.volume = pVoloume;
      
        soundSource.mSoundSource.clip = mSoundClipStorage.GetClip(pClipKey);
        soundSource.mSoundSource.Play();
        mSoundList.Add(soundSource);
        soundSource.StartCoroutine(PlayingSound(soundSource.mSoundSource.clip.length, soundSource));
    }


    public void StartTTS(string pName)
    {
        StartCoroutine(PlayTTSCo(pName));
    }

    public void StartEffect(string pName)
    {
        StartCoroutine(PlayEFFECTCo(pName));
    }


    //TTS 플레이
    public IEnumerator PlayTTSCo(string pName)
    {
        this.Play(pName, 1f);
        yield return new WaitForSeconds(SoundMgr.Inst.GetTTSSoundLength(pName) +0.6f);
    }

    //Effect 플레이
    public IEnumerator PlayEFFECTCo(string pName)
    {
        this.Play(pName, 0.6f);
        yield return new WaitForSeconds(SoundMgr.Inst.GetEffectSoundLength(pName));
    }

    //사운드 스탑. 돌고 있는 사운드의 이름을 찾아서 그 해당 사운드를 멈춥니다.
    public void Stop(string pClipKey)
    {
        SoundSource soundSource = Find(pClipKey);
        if (soundSource != null)
        {
            soundSource.mSoundSource.Stop();
            mSoundPooling.ReturnSoundSource(soundSource);
            mSoundList.Remove(soundSource);
            soundSource.gameObject.SetActive(false);
        }
    }

    //돌고있는 사운드 전부 꺼줍니다.
    public void AllStop()
    {
        for (int i = 0; i < mSoundList.Count; i++)
        {
            mSoundList[i].mSoundSource.Stop();
            mSoundPooling.ReturnSoundSource(mSoundList[i]);
            mSoundList[i].gameObject.SetActive(false);
        }
        mSoundList.Clear();
    }

    //TTS사운드 길이 반환
    public float GetTTSSoundLength(string pClip)
    {
        AudioClip ac = mSoundClipStorage.GetClip(pClip);
        return ac.length;
    }

   //Effect사운드 길이 반환
    public float GetEffectSoundLength(string pClip)
    {
        AudioClip ac = mSoundClipStorage.GetClip(pClip);
        return ac.length;
    }


    //사운드 페이드 아웃
    public void AllFadeOut(float pMaxTime = 2f)
    {
        StartCoroutine(FadeOutSoud(pMaxTime));
    }

    //사운드 페이드 아웃 기능
    private IEnumerator FadeOutSoud(float pMaxTime)
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            for (int i = 0; i < mSoundList.Count; i++)
            {
                SoundSource ss = mSoundList[i];
                ss.mSoundSource.volume = Mathf.Lerp(1, 0, time / pMaxTime);
            }

            if (time >= pMaxTime) break;
            yield return null;
        }
        AllStop();
    }

    //사운드 썼으면 돌려주세요..잘 썼습니다
    private IEnumerator PlayingSound(float pMaxTime, SoundSource pAudioSource)
    {
        yield return new WaitForSeconds(pMaxTime);

        mSoundPooling.ReturnSoundSource(pAudioSource);
        mSoundList.Remove(pAudioSource);
        pAudioSource.gameObject.SetActive(false);
    }

    //사운드 멈추려면 돌고있는 사운드 이름 찾아야 하지 않을까요
    public SoundSource Find(string pKey)
    {
        for (int i = 0; i < mSoundList.Count; i++)
        {
            SoundSource ss = mSoundList[i];

            if (ss.mSoundSource.clip.name == pKey)
            {
                return ss;
            }
        }
        return null;
    }

    //사운드 켜진거있니
    public bool IsPlaySound()
    {
        if (GetPlaySoundCont() > 0) { return true; }
        else return false;
    }

    //사운드 재생되는거 몇개니
    public int GetPlaySoundCont()
    {
        int count = 0;
        for (int i = 0; i < mSoundList.Count; i++)
        {
            if (mSoundList[i].mSoundSource.isPlaying)
            {
                count++;
            }
        }
        return count;
    }

    //시퀀스 종료시 걍 한번 호출해주세요
    public void StopSoundSystem()
    {
        AllStop();
        StopAllCoroutines();
    }


    //사운드 페이드해서 사라짐
    public void StartFadeOutSound(float pTime)
    {
        if (GetPlaySoundCont() > 0)
        {
            StartCoroutine(Fade(pTime));
        }
    }

    //사운드 페이드 기능
    private IEnumerator Fade(float pTime)
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            mAudioBGM.volume = Mathf.Lerp(mAudioBGM.volume, 0, time / pTime);

            for (int i = 0; i < mSoundList.Count; i++)
            {
                SoundSource ss = mSoundList[i];
                ss.mSoundSource.volume = Mathf.Lerp(ss.mSoundSource.volume, 0, time / pTime);
            }
            if (time >= pTime) break;
            yield return null;
        }

        this.AllStop();
    }

}