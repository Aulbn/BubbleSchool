using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationEnum {Throw}

public class PlayerAnimation : MonoBehaviour
{
    public Animator PlayerAnimator;
    private PlayerController PlayerController;
    private AnimationEnum PlayerAnimations;
    private bool IsRunning;
    private bool IsAiming;


    void Start()
    {
        PlayerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        SetRunAnimation();
    }


    public void SetRunAnimation()
    {

        if (PlayerController.MoveInput.magnitude != 0)
            PlayerAnimator.SetBool("IsRunning", true);
        else
            PlayerAnimator.SetBool("IsRunning", false);

    }

    public void SetAimAnimation(bool isAiming)
    {
        PlayerAnimator.SetBool("IsAiming", isAiming);
    }

    public void ThrowAnimation()
    {
        PlayerAnimator.SetTrigger("Throw");
    }

    public void StabAnimation()
    {
        PlayerAnimator.SetTrigger("Stab");
    }

}
