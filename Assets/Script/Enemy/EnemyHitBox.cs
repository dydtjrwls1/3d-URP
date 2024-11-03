using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public event Action<HealthPresenter> onHit = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            HealthPresenter health = other.GetComponent<HealthPresenter>();
            if (health != null)
            {
                onHit?.Invoke(health);
            }
        }
    }
}
