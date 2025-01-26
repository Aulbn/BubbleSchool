using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // private Camera _Camera;
    private PlayerController _Player;
    private Vector3 _OriginalPosition;
    public float FollowSpeed = 1;
    public float FollowPercentage = 0.2f;

    private void Awake()
    {
        // _Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        _Player = PlayerController.Instance;
        _OriginalPosition = transform.position;
    }

    void Update()
    {
        var playerPos = _Player.transform.position;
        var offsetPos = Vector3.Lerp(_OriginalPosition, playerPos, FollowPercentage);
        transform.position = Vector3.Lerp(transform.position, offsetPos, Time.deltaTime * FollowSpeed);
    }
}
