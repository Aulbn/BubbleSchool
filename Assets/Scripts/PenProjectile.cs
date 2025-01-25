using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenProjectile : MonoBehaviour
{
    public LayerMask WallLayer;
    public LayerMask EnemyLayer;
    public float ThrowSpeed;
    public float ThrowHitBoxSize;
    public Collider PickUpCollider;

    public void Equip(PlayerController player)
    {
        transform.parent = player.WeaponJoint;
        player.HasPen = true;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        PickUpCollider.enabled = false;
    }
    
    public bool Throw(Vector3 startPos, Vector3 dir)
    {
        transform.parent = null;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        
        if (Physics.Raycast(startPos, dir, out var hit, 50, WallLayer))
        {
            StartCoroutine(IEThrowTravel(startPos,  hit.point));
            return true;
        }

        return false;
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
            var rayDir = lastPos - transform.position;
            
            var brokenBubblesThisFrame = BreakBubbleRay(transform.position, lastPos);
            if (brokenBubblesThisFrame > 0)
            {
                brokenBubbles += brokenBubblesThisFrame;
                UIManager.ShowMultiplier(brokenBubbles);
            }

            lastPos = transform.position;
            yield return null;
        } while (distance < 1);
        
        // brokenBubblesThisFrame = BreakBubbleRay(transform.position, lastPos);
        // if (brokenBubblesThisFrame > 0)
        // {
        //     brokenBubbles += brokenBubblesThisFrame;
        //     UIManager.ShowMultiplier(brokenBubbles);
        // }        
        
        transform.position = endPos;
        PickUpCollider.enabled = true;
    }

    private int BreakBubbleRay(Vector3 startPos, Vector3 endPos)
    {
        var rayDir = endPos - startPos;
        int brokenBubbles = 0;

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
                    brokenBubbles++;
                }
            }
        }

        return brokenBubbles;
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
