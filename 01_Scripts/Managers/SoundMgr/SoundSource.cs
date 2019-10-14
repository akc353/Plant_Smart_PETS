using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour {

    public AudioSource mSoundSource;

    private void Awake()
    {
        mSoundSource = this.GetComponent<AudioSource>();
    }

}