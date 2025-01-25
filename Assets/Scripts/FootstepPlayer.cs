using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] FootstepClips;
    public Vector2 PitchRange;
    public Vector2 VoulmeRange;
    
    public void Footstep()
    {
        Source.volume = Random.Range(VoulmeRange.x, VoulmeRange.y);
        Source.pitch = Random.Range(PitchRange.x, PitchRange.y);
        Source.PlayOneShot(FootstepClips[UnityEngine.Random.Range(0, FootstepClips.Length-1)]);
    }
}
