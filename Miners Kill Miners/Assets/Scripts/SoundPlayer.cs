using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundPlayer : MonoBehaviour {

    public static SoundPlayer instance;
    List<AudioSource> soundList;

    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void Play(AudioClip soundToPlay)
    {
        AudioSource[] srcs = GetComponents<AudioSource>();
        AudioSource freeSourcePlayer = null;

        if (srcs != null)
        {
            for (int i = 0; i < srcs.Length; i++)
            {
                if (!srcs[i].isPlaying)
                {
                    freeSourcePlayer = srcs[i];
                    break;
                }
            }
        }
        if(freeSourcePlayer == null)
        {
            freeSourcePlayer = gameObject.AddComponent<AudioSource>();
        }

        freeSourcePlayer.clip = soundToPlay;
        freeSourcePlayer.Play();
    }

    void Start()
    {

    }

}
