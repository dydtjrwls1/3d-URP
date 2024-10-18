using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;

    float m_CurrentHealth;

    bool IsAlive => m_CurrentHealth > 0.0f;

    public float CurrentHealth
    {
        get => m_CurrentHealth;
        private set
        {
            if (IsAlive)
            {
                m_CurrentHealth = value;
            }
        }
    }

    private void Start()
    {
        m_CurrentHealth = maxHealth;
    }

    public float HealthRatio => m_CurrentHealth / maxHealth;

    public void OnDamage(float damage) 
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0.0f)
        {
            OnDie();
        }
    }

    private void OnDie()
    {
        gameObject.SetActive(false);
    }
}
