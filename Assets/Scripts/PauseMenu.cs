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

    private void Update()
    {
        Mixer.SetFloat("Main", VolumeSlider.value);
        Mixer.SetFloat("Music", MusicVolumeSlider.value);
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
