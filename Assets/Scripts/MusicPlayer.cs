using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource Source_Intro;
    private AudioSource Source_Loop;
    public AudioClip IntroClip;
    public AudioClip LoopClip;
    public AudioMixerGroup MixerGroup;
    public AudioMixerGroup MixerGroup_Loop;
    

    private void Awake()
    {
        Source_Intro = gameObject.AddComponent<AudioSource>();
        Source_Intro.clip = IntroClip;
        Source_Intro.playOnAwake = false;
        Source_Intro.outputAudioMixerGroup = MixerGroup;
        
        Source_Loop = gameObject.AddComponent<AudioSource>();
        Source_Loop.outputAudioMixerGroup = MixerGroup_Loop;
        Source_Loop.clip = LoopClip;
        Source_Loop.loop = true;
        Source_Loop.playOnAwake = false;
    }

    private void Start()
    {
        Source_Intro.Play();
    }

    private void Update()
    {
        if (Source_Loop.isPlaying == false && Source_Intro.time >= Source_Intro.clip.length - 0.2f)
        {
            // Source_Intro.Stop();
            Debug.Log("Play LOOP!");
            Source_Loop.Play();
        }
    }
}
