using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PenProjectile : MonoBehaviour
{
    public LayerMask WallLayer;
    public LayerMask EnemyLayer;
    public float ThrowSpeed;
    public float ThrowHitBoxSize;
    public Collider PickUpCollider;
    public Animator Animator;

    [Header("Sounds")]
    public AudioSource WallHitSound;
    public AudioSource PickUpSound;

    private void Start()
    {
        transform.parent = null;
    }

    public void Equip(PlayerController player)
    {
        // transform.parent = player.WeaponJoint;
        player.HasPen = true;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        PickUpCollider.enabled = false;
        PickUpSound.Play();
    }

    public bool Throw(Vector3 startPos, Vector3 dir)
    {
        // transform.parent = null;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        if (Physics.Raycast(startPos, dir, out var hit, 50, WallLayer))
        {
            StartCoroutine(IEThrowTravel(startPos, hit.point));
            return true;
        }

        return false;
    }

    private void PlayHitAnimation()
    {
        Animator.SetTrigger("HitWall");
    }

    private IEnumerator IEThrowTravel(Vector3 startPos, Vector3 endPos)
    {
        float distance = 0;
        Vector3 lastPos = startPos;
        int brokenBubbles = 0;
        // var brokenBubblesThisFrame = 0;

        do
        {
            transform.position = Vector3.Lerp(startPos, endPos, distance);
            distance += Time.deltaTime * ThrowSpeed;
            // var rayDir = lastPos - transform.position;

            var brokenBubblesThisFrame = BreakBubbleRay(transform.position, lastPos);
            if (brokenBubblesThisFrame.Length > 0)
            {
                brokenBubbles += brokenBubblesThisFrame.Length;
                // GameManager.AddMultiplierScore(10, brokenBubbles);
                foreach (var pos in brokenBubblesThisFrame)
                {
                    GameManager.AddMultiplierScore(10, brokenBubbles, pos);
                }
            }

            lastPos = transform.position;
            yield return null;
        } while (distance < 1);

        transform.position = endPos + (startPos - endPos).normalized * 0.2f;
        PickUpCollider.enabled = true;
        PlayHitAnimation();
        PlaySound_WallHit();
    }

    private void PlaySound_WallHit()
    {
        WallHitSound.pitch = Random.Range(1f, 2f);
        WallHitSound.Play();
    }

    private Vector3[] BreakBubbleRay(Vector3 startPos, Vector3 endPos)
    {
        var rayDir = endPos - startPos;
        // int brokenBubbles = 0;
        List<Vector3> brokenBubbles = new List<Vector3>();

        // var hits = Physics.RaycastAll(startPos, rayDir.normalized, rayDir.magnitude, EnemyLayer);
        var hits = Physics.SphereCastAll(startPos, ThrowHitBoxSize, rayDir.normalized, rayDir.magnitude, EnemyLayer);
        if (hits != null)
        {
            // Debug.Log("Throw hits " + hits.Length);
            foreach (var hit in hits)
            {
                var student = hit.transform.gameObject.GetComponent<Student>();
                if (student.State == Student.StudentState.Blowing)
                {
                    student.BreakBubble();
                    brokenBubbles.Add(student.transform.position + Vector3.up * 2);
                    // brokenBubbles++;
                }
            }
        }

        return brokenBubbles.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            Equip(player);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ThrowHitBoxSize);
    }
}
