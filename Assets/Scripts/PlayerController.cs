using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Stunned,
        Idle, 
        Melee,
        Throwing
    }

    public PlayerState State;
    
    public float MovementSpeed = 1f;
    public float RotationSpeed = 1f;

    [Header("Violence")] 
    public float StabMoveLockTime;
    public SphereCollider StabCollider;
    public LayerMask EnemyLayer;
    public Transform WeaponJoint;
    public PenProjectile Pen;
    public bool HasPen;
    
    private Vector2 _MovementInput;
    private Quaternion _CurrentRotation;
    private CharacterController _Cc;
    private PlayerAnimation _Animation;

    public Vector2 MoveInput => _MovementInput; 

    private void Awake()
    {
        _Cc = GetComponent<CharacterController>();
        _Animation = GetComponent<PlayerAnimation>();
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
                
                if (Input.GetKeyDown(KeyCode.Mouse0) && HasPen)
                {
                    Stab();
                }
                if (Input.GetKeyDown(KeyCode.Mouse1) && HasPen)
                {
                    StartThrow();
                }
                break;            
            case PlayerState.Melee:
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation, Time.deltaTime * RotationSpeed * 10);
                break;            
            case PlayerState.Throwing:
                
                _CurrentRotation =  Quaternion.LookRotation(GetDirectionFromCursor(), Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation,
                    Time.deltaTime * RotationSpeed);
                
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    Throw();
                }
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    State = PlayerState.Idle;
                    _Animation.SetAimAnimation(false);
                }
                
                break;            
        }
    }

    private void Stab()
    {
        // Debug.Log("Stab");
        
        _CurrentRotation =  Quaternion.LookRotation(GetDirectionFromCursor(), Vector3.up);

        var hits = Physics.OverlapSphere(StabCollider.transform.position, StabCollider.radius, EnemyLayer);
        // Debug.Log("Found " + hits.Length + " colliders");
        foreach (var hit in hits)
        {
            // Debug.Log("HIT! " + hit.transform.gameObject + " (" + hit.transform.gameObject.tag + ")");
            if (hit.transform.gameObject.tag.Equals("Enemy"))
            {
                Student student = hit.transform.gameObject.GetComponent<Student>();
                if (student.State == Student.StudentState.Blowing)
                {
                    student.BreakBubble();
                    GameManager.AddScore(10);
                }
            }
        }
        
        StartCoroutine(IEStab());
    }

    private void StartThrow()
    {
        if (!HasPen)
            return;
        
        Debug.Log("Start Throw");
        _Animation.SetAimAnimation(true);
        State = PlayerState.Throwing;
    }
    
    private void Throw()
    {
        if (!HasPen)
            return;
        
        Debug.Log("Throw");
        var throwDir = GetDirectionFromCursor();
        Pen.Throw(WeaponJoint.position, throwDir);
        State = PlayerState.Idle;
        HasPen = false;
        _Animation.SetAimAnimation(false);
        _Animation.ThrowAnimation();
    }
    
    private Vector3 GetDirectionFromCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        Plane plane = new Plane(Vector3.up, transform.position + Vector3.up);
        Ray ray = GameManager.Instance.Camera.ScreenPointToRay(mousePos);
        plane.Raycast(ray, out var magnitude);
        // Vector3 
        // Vector3 playerScreenPos = GameManager.Instance.Camera.WorldToScreenPoint(transform.position);
        // var dir = (mousePos - playerScreenPos).normalized;
        Vector3 hitPoint = ray.origin + ray.direction * magnitude;
        hitPoint.y = 0;
        var dir = (hitPoint - transform.position).normalized;
        Debug.DrawLine(GameManager.Instance.Camera.transform.position, hitPoint);
        return dir;
    }

    // private Vector3 GetDirectionFromCursor()
    // {
    //     Vector3 mousePos = Input.mousePosition;
    //     Vector3 playerScreenPos = GameManager.Instance.Camera.WorldToScreenPoint(transform.position);
    //     var dir = (mousePos - playerScreenPos).normalized;
    //     return new Vector3(dir.x, 0, dir.y);
    // }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(StabCollider.bounds.center, StabCollider.radius);
    }
}
