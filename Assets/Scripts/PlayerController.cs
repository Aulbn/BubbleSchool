using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private bool _StartStatic;

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
    public GameObject Reticle;

    [Header("Sounds")]
    public AudioSource ThrowSoundSource;
    public AudioSource StabSoundSource;

    private Vector2 _MovementInput;
    private Quaternion _CurrentRotation;
    private CharacterController _Cc;
    private PlayerAnimation _Animation;
    private PlayerInput _PlayerInput;


    public Vector2 MoveInput => _MovementInput;
    public static bool MeleeThisFrame;
    public static bool ThrowDownThisFrame;
    public static bool ThrowUpThisFrame;
    public static bool GamepadNorthThisFrame;
    public static bool GamepadEastThisFrame;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _Cc = GetComponent<CharacterController>();
        _Animation = GetComponent<PlayerAnimation>();
        _PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        State = PlayerState.Idle;
        Reticle.SetActive(false);

        if (_StartStatic)
            State = PlayerState.Stunned;
    }

    private void Update()
    {
        MeleeThisFrame = _PlayerInput.actions["Melee"].WasPressedThisFrame();
        ThrowDownThisFrame = _PlayerInput.actions["Throw"].WasPressedThisFrame();
        ThrowUpThisFrame = _PlayerInput.actions["Throw"].WasReleasedThisFrame();
        GamepadNorthThisFrame = _PlayerInput.actions["GamepadNorth"].WasReleasedThisFrame();
        GamepadEastThisFrame = _PlayerInput.actions["GamepadEast"].WasReleasedThisFrame();

        var movementDir = new Vector3(_MovementInput.x, 0, _MovementInput.y).normalized;

        if (Time.deltaTime == 0)
            return;

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

                if (MeleeThisFrame && HasPen)
                {
                    Stab();
                }
                if (ThrowDownThisFrame && HasPen)
                {
                    StartThrow();
                }
                break;
            case PlayerState.Melee:
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation, Time.deltaTime * RotationSpeed * 10);
                break;
            case PlayerState.Throwing:

                _CurrentRotation = Quaternion.LookRotation(GetDirectionFromCursor(), Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, _CurrentRotation,
                    Time.deltaTime * RotationSpeed);

                Reticle.transform.rotation = _CurrentRotation;

                if (ThrowUpThisFrame)
                {
                    Throw();
                }

                if (MeleeThisFrame)
                {
                    State = PlayerState.Idle;
                    _Animation.SetAimAnimation(false);
                    Reticle.SetActive(false);
                }

                break;
        }
    }

    private void LateUpdate()
    {
        if (HasPen)
        {
            Pen.transform.position = WeaponJoint.position;
            Pen.transform.rotation = WeaponJoint.rotation;
        }
    }

    public void PauseGame()
    {
        PauseMenu.Toggle();
    }

    private void PlayThrowSound()
    {
        ThrowSoundSource.pitch = Random.Range(1f, 1.5f);
        ThrowSoundSource.Play();
    }

    private void PlayStabSound()
    {
        StabSoundSource.pitch = Random.Range(1f, 2f);
        StabSoundSource.Play();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _MovementInput = ctx.ReadValue<Vector2>();
    }

    private void Stab()
    {
        // Debug.Log("Stab");

        _CurrentRotation = Quaternion.LookRotation(GetDirectionFromCursor(), Vector3.up);
        _Animation.StabAnimation();
        PlayStabSound();

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
                    GameManager.AddScore(5, student.transform.position + Vector3.up * 2);
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
        Reticle.SetActive(true);
    }

    private void Throw()
    {
        if (!HasPen)
            return;

        Debug.Log("Throw");
        PlayThrowSound();
        var throwDir = GetDirectionFromCursor();
        State = PlayerState.Idle;
        HasPen = false;
        _Animation.SetAimAnimation(false);
        _Animation.ThrowAnimation();
        Reticle.SetActive(false);
        Pen.Throw(transform.position + (throwDir.normalized * 0.5f) + Vector3.up, throwDir);
    }

    private Vector3 GetDirectionFromCursor()
    {
        Debug.Log(_PlayerInput.currentControlScheme);

        if (_PlayerInput.currentControlScheme.Equals("Gamepad"))
        {
            return new(_MovementInput.x, 0, _MovementInput.y);
        }

        Plane plane = new Plane(Vector3.up, transform.position + Vector3.up);
        Ray ray = GameManager.Instance.Camera.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var magnitude);
        Vector3 hitPoint = ray.origin + ray.direction * magnitude;
        hitPoint.y = 0;
        var dir = (hitPoint - transform.position).normalized;
        Debug.DrawLine(GameManager.Instance.Camera.transform.position, hitPoint);
        return dir;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(StabCollider.bounds.center, StabCollider.radius);
    }
}
