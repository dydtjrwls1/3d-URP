using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public event Action<Health> onHit = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                onHit?.Invoke(health);
            }
        }
    }
}
