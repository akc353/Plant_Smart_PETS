using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//오디오 소스를 가지고 있는 SoundSource를 풀링 할 클래스.
public class SoundPooling
{
    private Stack<SoundSource> mSoundPool = new Stack<SoundSource>();
    //private List<AudioSource> mSoundList = new List<AudioSource>();
    private int mSoundIdx=0;

    public bool HasSoundSource()
    {
        return mSoundPool.Count > 0;
    }

    public SoundSource GetSoundSource()
    { 
        return mSoundPool.Pop();
    }   
   
    public SoundSource CreateSoundSource(Transform pParentTr)
    {
        GameObject goSound = new GameObject("Sound"+mSoundIdx.ToString());
        mSoundIdx++;
        goSound.transform.SetParent(pParentTr);
        SoundSource audioSource = goSound.AddComponent<SoundSource>();
      //  mSoundList.Add(audioSource);
        
        return audioSource;
    }

    public void ReturnSoundSource(SoundSource pAudioSource)
    {
        mSoundPool.Push(pAudioSource);
      
    }
}
