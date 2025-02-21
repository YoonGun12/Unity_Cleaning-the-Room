using System;
using UnityEngine;

public class KickCollision : MonoBehaviour
{
    private float kickForce = 50f;
    [SerializeField] private GameObject dustPrefab;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destructible"))
        {
            
            Rigidbody enemyRigid = other.GetComponent<Rigidbody>();
            if (enemyRigid != null)
            {
                Vector3 contactPoint = other.ClosestPoint(transform.position);
                Vector3 forceDirection = contactPoint - transform.position;
                enemyRigid.AddForce(forceDirection.normalized * kickForce, ForceMode.Impulse);
                Instantiate(dustPrefab, contactPoint, Quaternion.identity);
            }
        }
    }
}
