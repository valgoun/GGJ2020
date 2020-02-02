using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float ImpactForce = 15f;
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<IDamageable>()?.Damage(transform.forward * ImpactForce);
        Destroy(gameObject);
    }
}
