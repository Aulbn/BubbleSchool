using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenProjectile : MonoBehaviour
{
    public LayerMask WallLayer;
    public float ThrowSpeed;
    public Collider PickUpCollider;

    public void Equip(Transform weaponJoint)
    {
        transform.parent = weaponJoint;
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
        while (distance < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, distance);
            distance += Time.deltaTime * ThrowSpeed;
            yield return null;
        }

        transform.position = endPos;
        PickUpCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Equip(other.GetComponent<PlayerController>().WeaponJoint);
        }
    }
}
