using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _Instance;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI HighscoreText;
    [Header("Popup")]
    public TextMeshProUGUI PopupText;
    public AnimationCurve PopupAnimationCurve;
    public float PopupAnimationStrength = 10;
    public float PopupAnimationTime = 1f;
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

    private void OnEnable()
    {
        HighscoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore", 0);
        _Instance.ScoreText.text = "Score: " + 0;
    }

    public static void SetTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        string str = time.ToString(@"mm\:ss");
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

    public static void ShowPopupScore(Vector3 worldPosition, int score, int multiplier = 1)
    {
        // var screenPosition = CameraController.Instance._Camera.WorldToViewportPoint(worldPosition);
        var screenPosition = RectTransformUtility.WorldToScreenPoint(CameraController.Instance._Camera, worldPosition);
        var popupItem = Instantiate(_Instance.PopupText.gameObject, _Instance.transform);
        popupItem.SetActive(true);
        var rect = popupItem.GetComponent<RectTransform>();
        rect.anchoredPosition = _Instance.transform.InverseTransformPoint(screenPosition);

        var tmp = popupItem.GetComponent<TextMeshProUGUI>();
        tmp.text = score.ToString();
        tmp.color = _Instance.MultiplierColors[Mathf.Clamp(multiplier - 1, 0, _Instance.MultiplierColors.Length - 1)];
        _Instance.StartCoroutine(_Instance.IEPopupScore(rect, worldPosition));
    }

    private IEnumerator IEPopupScore(RectTransform rect, Vector3 worldPosition)
    {
        float timer = 0;
        var tmp = rect.GetComponent<TextMeshProUGUI>();
        var originalSize = tmp.fontSize;

        do
        {
            var screenPosition = RectTransformUtility.WorldToScreenPoint(CameraController.Instance._Camera, worldPosition);
            rect.anchoredPosition = _Instance.transform.InverseTransformPoint(screenPosition);

            // var newPos = originPosition;
            // newPos.y = originPosition.y + PopupAnimationCurve.Evaluate(timer / PopupAnimationTime) * PopupAnimationStrength;
            // rect.anchoredPosition = originPosition;

            tmp.fontSize = originalSize * PopupAnimationCurve.Evaluate(timer / PopupAnimationTime) * PopupAnimationStrength;

            timer += Time.deltaTime;
            yield return null;
        } while (timer < PopupAnimationTime);

        Destroy(rect.gameObject);
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
