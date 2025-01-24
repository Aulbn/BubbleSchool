using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Student> StudentList;

    public Vector2 MinMaxBubbleCooldown;
    public float NextBubbleTimer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        StudentList = new List<Student>();
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
                    student.BlowBubble(3f);
                    break;
                }
            }
            NextBubbleTimer = Random.Range(MinMaxBubbleCooldown.x, MinMaxBubbleCooldown.y);
        }

        NextBubbleTimer -= Time.deltaTime;
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
