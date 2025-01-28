using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentAnimation : MonoBehaviour
{

    public Animator StudentAnimator;
    private float _IdleOffset;

    void Start()
    {
        _IdleOffset = Random.Range(0f, 1f);

        StudentAnimator.SetFloat("IdleOffset", _IdleOffset);

    }

    public void Play_BlowBubble()
    {
        StudentAnimator.SetTrigger("BlowBubble");
    }
    
    public void Play_Stunned(bool isStunned)
    {
        StudentAnimator.SetBool("IsStunned", isStunned);
    }



}
