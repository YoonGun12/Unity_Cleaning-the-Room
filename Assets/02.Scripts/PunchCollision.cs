using System;
using UnityEngine;


public class PunchCollision : MonoBehaviour
{
    private float punchForce = 15f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destrictible"))
        {
            Rigidbody enemyRigid = other.GetComponent<Rigidbody>();
            if (enemyRigid != null)
            {
                Vector3 contactPoint = other.ClosestPoint(transform.position);
                Vector3 forceDirection = contactPoint - transform.position;
                enemyRigid.AddForce(forceDirection.normalized * punchForce, ForceMode.Impulse);
            }
        }
    }
}
