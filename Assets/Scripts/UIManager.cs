using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _Instance;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    [Header("Multiplier")]
    public TextMeshProUGUI MultiplierText;
    public AnimationCurve MultiplierAnimationCurve;
    public float MultiplierAnimationSpeed;
    public Color[] MultiplierColors;
    
    private Coroutine MultiplierCoroutine;
    private float MiltiplierOriginalSize;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _Instance.MultiplierText.enabled = false;
        MiltiplierOriginalSize = MultiplierText.fontSize;
    }

    public static void SetTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        
        string str = time .ToString(@"mm\:ss");
        _Instance.TimeText.text = str;
    }
    
    public static void SetScore(int score)
    {
        _Instance.ScoreText.text = "Score: " + score;
    }

    public static void ShowMultiplier(int comboCount)
    {
        _Instance.MultiplierText.text = "x" + comboCount;
        _Instance.MultiplierText.color = _Instance.MultiplierColors[Mathf.Clamp(comboCount - 1, 0, _Instance.MultiplierColors.Length - 1)];
        
        if (_Instance.MultiplierCoroutine != null)
            _Instance.StopCoroutine(_Instance.MultiplierCoroutine);
        _Instance.MultiplierCoroutine = _Instance.StartCoroutine(_Instance.IEShowMultiplier(comboCount));
    }

    private IEnumerator IEShowMultiplier(int comboCount)
    {
        _Instance.MultiplierText.enabled = true;
        float timer = 0;
        while (timer < 1)
        {
            MultiplierText.fontSize = MiltiplierOriginalSize * MultiplierAnimationCurve.Evaluate(timer) * (comboCount > 1 ? comboCount * 1.3f : 1);
            timer += Time.deltaTime * MultiplierAnimationSpeed;
            yield return null;
        }
        MultiplierText.fontSize = MiltiplierOriginalSize * MultiplierAnimationCurve.Evaluate(1);
        _Instance.MultiplierText.enabled = false;
    }
}
