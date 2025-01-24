using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _Instance;

    public TextMeshProUGUI ScoreText;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    public static void SetScore(int score)
    {
        _Instance.ScoreText.text = "Score: " + score;
    }
}
