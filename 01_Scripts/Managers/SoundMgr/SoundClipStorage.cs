using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

//클립매니저 없앰. 클립 매니저를 대신하는 역할 정도.
public class SoundClipStorage
{
    private Dictionary<string, AudioClip> mSoundClipDic = null;

    //혹시 경로 다를까봐
    public SoundClipStorage(string pPath, string pPath2)
    {
        mSoundClipDic = ResourceLoad(pPath,pPath2);
    }
	
    //클립얻어온다.
    public AudioClip GetClip(string pKey)
    {
        if (mSoundClipDic.ContainsKey(pKey))
        {
            return mSoundClipDic[pKey];
        }
        else
        {
            Debug.Log(pKey + " : 사운드 접근 실패");
            return null;
        }
    }
    //오디오 클립 담을 사운드 클립 딕셔너리.
    //public Dictionary<string, T> mResrouceDic = new Dictionary<string, T>();

    //사운드 클립들 로드.
    //ResourcesSystem에서 저장했던 사운드 리스트들 (오디오클립) 담아줄것임.
    //JSONNODE.Parse는 문자열을 파싱해서 문자열로 된 객체를 생성합니다.
    //MR_ResourceSystem에서 사운드 파일경로를 찾아서 문자열로 저장할때 JSON식으로 저장했습니다.
    //JSON식으로 저장됐던 파일들 (문자열로 저장했던 사운드들)을 오디오 클립으로 변환 가져옵니다.
    public Dictionary<string, AudioClip> ResourceLoad(string pSoundListPath, string pSoundListPath2)
    {
        Dictionary<string, AudioClip> ResrouceDic = new Dictionary<string, AudioClip>();
        JSONNode jn = JSON.Parse(LoadFile(pSoundListPath));  //TTS
        JSONNode jn2 = JSON.Parse(LoadFile(pSoundListPath2));  //Effect


        Debug.Log("Json Count : " + jn.Count);
        Debug.Log("EffectJson Count : " + jn2.Count);

        for (int i = 0; i < jn.Count; i++)
        {
            string loadPath = jn[i]["path"].Value.Replace(".ogg", "").Replace(".wav", "").Replace("_", " ").Replace("ㄸ", "\"").Replace("ㅅ", ",");
            AudioClip soundClip = Resources.Load(loadPath) as AudioClip;

            if (soundClip != null)
            {
                // Debug.Log("로드 성공 : " + loadPath);
            }
            else
            {
                Debug.LogError("로드 실패 : " + loadPath);
            }

            if (ResrouceDic.ContainsKey(jn[i]))
            {
                Debug.Log("이미 있는 TTS입니다 :" + jn[i]["Key"].ToString());
                continue;
            }
            else
                ResrouceDic.Add(jn[i]["key"].Value, soundClip);

        }

        for (int i = 0; i < jn2.Count; i++)
        {
            string loadPath = jn2[i]["path"].Value.Replace(".ogg", "").Replace(".wav", "").Replace("_", " ").Replace("ㄸ", "\"").Replace("ㅅ", ",");
            AudioClip soundClip = Resources.Load(loadPath) as AudioClip;

            if (soundClip != null)
            {
                // Debug.Log("로드 성공 : " + loadPath);
            }
            else
            {
                Debug.LogError("로드 실패 : " + loadPath);
            }

            if (ResrouceDic.ContainsKey(jn2[i]))
            {
                Debug.Log("이미 있는 Effect입니다 :" + jn2[i]["Key"].ToString());
                continue;
            }
            else ResrouceDic.Add(jn2[i]["key"].Value, soundClip);
        }
        Debug.Log("딕셔너리 카운트 : " + ResrouceDic.Count);
        return ResrouceDic;
    }

    private string LoadFile(string pPath)
    {
        TextAsset ta = Resources.Load(pPath) as TextAsset;

        if (ta == null)
        {
            Debug.Log("파일 로드 실패 : " + pPath);
        }

        return ta.text;
    }
}
