using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour
{
    public GameObject BubbleGumMesh;
    public AnimationCurve TempBubbleBlowingCurve;
    public Vector2 MinMaxBubbleSize;
    public ParticleSystem Ps_BubblePop;

    public enum StudentState
    {
        None,
        Idle,
        Blowing
    }
    public StudentState State;
    
    private Coroutine _BubbleCoroutine;


    private void Start()
    {
        GameManager.AddStudent(this);
        State = StudentState.Idle;
        BubbleGumMesh.SetActive(false);
    }

    public void BlowBubble(float blowTime)
    {
        Debug.Log("Blow Bubble!" , gameObject);
        if (_BubbleCoroutine != null)
            StopCoroutine(_BubbleCoroutine);
        
        _BubbleCoroutine = StartCoroutine(IEBlowBubble(blowTime));
    }

    public void BreakBubble()
    {
        Debug.Log("BREAK BUBBLE" , gameObject);
        if (_BubbleCoroutine != null)
            StopCoroutine(_BubbleCoroutine);
        
        BubbleGumMesh.SetActive(false);
        State = StudentState.Idle;
        
        Ps_BubblePop.Play();
    }
    
    private IEnumerator IEBlowBubble(float blowTime)
    {
        State = StudentState.Blowing;
        float time = 0;
        Vector3 minSize = Vector3.one * MinMaxBubbleSize.x;
        Vector3 maxSize = Vector3.one * MinMaxBubbleSize.y;
        
        BubbleGumMesh.SetActive(true);
        
        while (time < blowTime)
        {
            float currentTime = TempBubbleBlowingCurve.Evaluate(time / blowTime);
            BubbleGumMesh.transform.localScale = Vector3.Lerp(minSize, maxSize, currentTime);
            time += Time.deltaTime;
            yield return null;
        }
        BubbleGumMesh.SetActive(false);
        State = StudentState.Idle;
    }


}
