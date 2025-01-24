using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 1f;
    public float RotationSpeed = 1f;

    [Header("Violence")] 
    public float StabMoveLockTime;
    public SphereCollider StabCollider;
    
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
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Stab();
                }
                break;            
            case PlayerState.Melee:
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation, Time.deltaTime * RotationSpeed * 10);
                break;            
            case PlayerState.Throwing:
                break;            
        }


    }

    private void Stab()
    {
        //Debug.Log("Stab");
        
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPos = GameManager.Instance.Camera.WorldToScreenPoint(transform.position);
        var dir = (mousePos - playerScreenPos).normalized;
        var lookDir = new Vector3(dir.x, 0, dir.y);
        _CurrentRotation =  Quaternion.LookRotation(lookDir, Vector3.up);

        var hits = Physics.SphereCastAll(StabCollider.center, StabCollider.radius, lookDir.normalized, 0);
        //Debug.Log("Found " + hits.Length + " colliders");
        foreach (var hit in hits)
        {
            //Debug.Log("HIT! " + hit.transform.gameObject + " (" + hit.transform.gameObject.tag + ")");
            if (hit.transform.gameObject.tag.Equals("Enemy"))
            {
                Student student = hit.transform.gameObject.GetComponent<Student>();
                if (student.State == Student.StudentState.Blowing)
                    student.BreakBubble();
            }
        }
        
        StartCoroutine(IEStab());
    }

    private IEnumerator IEStab()
    {
        State = PlayerState.Melee;
        float timer = 0;
        while (timer < StabMoveLockTime)
        {
            State = PlayerState.Melee;
            timer += Time.deltaTime;
            yield return null;
        }
        State = PlayerState.Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(StabCollider.bounds.center, StabCollider.radius);
    }
}
