using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public Camera _Camera;
    private PlayerController _Player;
    private Vector3 _OriginalPosition;
    public float FollowSpeed = 1;
    public float FollowPercentage = 0.2f;
    private float _FollowStrength = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static void SetFollowStrength(float value)
    {
        Instance._FollowStrength = value;
    }

    private void Start()
    {
        _Player = PlayerController.Instance;
        _OriginalPosition = transform.position;
    }

    void Update()
    {
        var playerPos = _Player.transform.position;
        var offsetPos = Vector3.Lerp(_OriginalPosition, playerPos, FollowPercentage * _FollowStrength);
        transform.position = Vector3.Lerp(transform.position, offsetPos, Time.deltaTime * FollowSpeed);
    }
}
