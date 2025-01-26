using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public AudioMixer Mixer;
    [Header("UI")]
    public Slider VolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider CameraFollowSlider;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Close();
    }

    private void OnEnable()
    {
        Mixer.GetFloat("MainVol", out float mainVol);
        VolumeSlider.value = Mathf.InverseLerp(-40, 0, mainVol);
        
        Mixer.GetFloat("MusicVol", out float musicVol);
        MusicVolumeSlider.value = Mathf.InverseLerp(-40, 0, musicVol);
    }

    private void Update()
    {
        Mixer.SetFloat("MainVol", Mathf.Lerp(-40, 0, VolumeSlider.value));
        Mixer.SetFloat("MusicVol", Mathf.Lerp(-40, 0, MusicVolumeSlider.value));
    }

    public static void Toggle()
    {
        if (Instance.gameObject.activeSelf)
            Close();
        else
            Open();
    }

    public static void Open()
    {
        Instance.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public static void Close()
    {
        Instance.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
