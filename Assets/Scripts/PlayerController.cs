using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 1f;
    public float RotationSpeed = 1f;

    [Header("Violence")] public float StabMoveLockTime;
    
    private Vector2 _MovementInput;
    private Quaternion _CurrentRotation;
    private CharacterController _Cc;

    public enum PlayerState
    {
        Stunned,
        Idle, 
        Melee,
        Throwing
    }

    public PlayerState State;

    private void Awake()
    {
        _Cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        State = PlayerState.Idle;
    }

    private void Update()
    {
        _MovementInput = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
            _MovementInput.x -= 1;
        if (Input.GetKey(KeyCode.D))
            _MovementInput.x += 1;
        if (Input.GetKey(KeyCode.W))
            _MovementInput.y += 1;
        if (Input.GetKey(KeyCode.S))
            _MovementInput.y -= 1;

        var movementDir = new Vector3(_MovementInput.x, 0, _MovementInput.y).normalized;

        
        switch (State)
        {
            case PlayerState.Stunned:
                break;
            case PlayerState.Idle:
                _Cc.Move(movementDir * (MovementSpeed * Time.deltaTime));
                if (_MovementInput != Vector2.zero)
                    _CurrentRotation = Quaternion.LookRotation(movementDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation,
                    Time.deltaTime * RotationSpeed);
                break;            
            case PlayerState.Melee:
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation,
                    Time.deltaTime * RotationSpeed);
                break;            
            case PlayerState.Throwing:
                break;            
        }


    }

    private IEnumerator IEStab()
    {
        float timer = 0;
        while (timer < StabMoveLockTime)
        {
            State = PlayerState.Melee;
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
