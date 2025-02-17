using System;
using UnityEngine;


public enum AttackType
{
    Punch,
    Kick,
    Special
}

public class KickCollision : MonoBehaviour
{
    private float kickForce = 100f;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Destrictible"))
        {
            Rigidbody enemyRigid = other.GetComponent<Rigidbody>();
            if (enemyRigid != null)
            {
                Vector3 contactPoint = other.ClosestPoint(transform.position);
                Vector3 forceDirection = contactPoint - transform.position;
                enemyRigid.AddForce(forceDirection.normalized * kickForce, ForceMode.Impulse);
            }
        }
    }
}
