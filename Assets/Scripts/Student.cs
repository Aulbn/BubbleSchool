using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Student : MonoBehaviour
{
    // public GameObject BubbleGumMesh;
    public AnimationCurve TempBubbleBlowingCurve;
    public Vector2 MinMaxBubbleSize;
    public ParticleSystem Ps_BubblePop;

    [Header("Animations")] private StudentAnimation _Animation;


    [Header("Sounds")] 
    public AudioSource PopSound;
    public AudioSource BlowSound;
    
    public enum StudentState
    {
        None,
        Idle,
        Chewing,
        Blowing,
        Stunned
    }
    public StudentState State;
    
    private Coroutine _BubbleCoroutine;


    private void Start()
    {
        GameManager.AddStudent(this);
        State = StudentState.Idle;
        _Animation = GetComponent<StudentAnimation>();
    }

    public void BlowBubble(float blowTime)
    {
        if (State == StudentState.Chewing || State == StudentState.Blowing)
            return;
        
        Debug.Log("Blow Bubble!" , gameObject);
        if (_BubbleCoroutine != null)
        {
            StopCoroutine(_BubbleCoroutine);
            State = StudentState.Idle;
        }
        
        _BubbleCoroutine = StartCoroutine(IEBlowBubble(blowTime));
    }

    public void BreakBubble()
    {
        Debug.Log("BREAK BUBBLE" , gameObject);
        if (_BubbleCoroutine != null)
            StopCoroutine(_BubbleCoroutine);
        
        State = StudentState.Stunned;
        BlowSound.Stop();
        Ps_BubblePop.Play();
        PlaySound_Pop();
        StartCoroutine(IEStun());
    }

    private IEnumerator IEStun()
    {
        _Animation.Play_Stunned(true);
        yield return new WaitForSeconds(3f);
        _Animation.Play_Stunned(false);
        State = StudentState.Idle;
    }
    
    private IEnumerator IEBlowBubble(float blowTime)
    {
        State = StudentState.Chewing;
        float time = 0;
        Vector3 minSize = Vector3.one * MinMaxBubbleSize.x;
        Vector3 maxSize = Vector3.one * MinMaxBubbleSize.y;
        
        _Animation.Play_BlowBubble();
        
        yield return new WaitForSeconds(2f);
        State = StudentState.Blowing;
        PlaySound_Blow();

        yield return new WaitForSeconds(4f);
        
        State = StudentState.Idle;
    }
    
    private void PlaySound_Pop()
    {
        PopSound.pitch = Random.Range(0.8f, 1.2f);
        PopSound.Play();
    }
    
    private void PlaySound_Blow()
    {
        BlowSound.pitch = Random.Range(1f, 1.5f);
        BlowSound.Play();
    }

    public void DodgePen()
    {
        _Animation.StudentAnimator.SetTrigger("Dodge");
    }
}
