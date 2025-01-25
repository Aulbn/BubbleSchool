using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    private static SoundSystem _Instance;

    public AudioSource Source;
    
    public static void PlayOneShot(AudioClip clip, float volume)
    {
        _Instance.Source.PlayOneShot(clip, volume);
    }
}
