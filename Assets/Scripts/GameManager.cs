using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Student> StudentList;
    public Camera Camera;

    public Vector2 MinMaxBubbleCooldown_Start;
    public Vector2 MinMaxBubbleCooldown_End;
    public Vector2 MinMaxBubbleBlowTime;
    public float NextBubbleTimer;
    public int Score;
    public float RoundTimer;
    private float _StartRoundTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        StudentList = new List<Student>();
    }

    private void Start()
    {
        _StartRoundTime = RoundTimer;
    }

    private void Update()
    {
        if (NextBubbleTimer <= 0)
        {
            var randomList = StudentList.ToArray();
            Shuffle(ref randomList);
            foreach (var student in randomList)
            {
                if (student.State == Student.StudentState.Idle)
                {
                    student.BlowBubble(Random.Range(MinMaxBubbleBlowTime.x, MinMaxBubbleBlowTime.y));
                    break;
                }
            }
            NextBubbleTimer = Random.Range(
                Mathf.Lerp(MinMaxBubbleCooldown_Start.x, MinMaxBubbleCooldown_End.x, Mathf.InverseLerp(1,0, RoundTimer / _StartRoundTime)), 
                Mathf.Lerp(MinMaxBubbleCooldown_Start.y, MinMaxBubbleCooldown_End.y, Mathf.InverseLerp(1,0, RoundTimer / _StartRoundTime))
                );
        }

        if (RoundTimer <= 0)
        {
            Time.timeScale = 0;
            return;
        }

        NextBubbleTimer -= Time.deltaTime;
        RoundTimer -= Time.deltaTime;
        UIManager.SetTime(RoundTimer);
    }

    public static void AddScore(int addedScore)
    {
        Instance.Score += addedScore;
        UIManager.SetScore(Instance.Score);
    }
    
    public static void AddMultiplierScore(int addedScore, int multiplier)
    {
        Instance.Score += addedScore * multiplier;
        UIManager.SetScore(Instance.Score);
        UIManager.ShowMultiplier(multiplier);
    }
    
    public static void Shuffle<T>(ref T[] array) {
        for (int i = array.Length - 1; i > 0; i--) {
            var r = UnityEngine.Random.Range(0, i + 1);
            if (r == i) continue;
            var temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }

    public static void AddStudent(Student student)
    {
        Instance.StudentList.Add(student);
    }
}
