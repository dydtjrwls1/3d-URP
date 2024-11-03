using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour, IInitialize
{
    public float maxHealth = 100.0f;

    float inverseMaxHealth;

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
                onHealthChange?.Invoke(m_CurrentHealth * inverseMaxHealth);

                // ü�¿� 0 �����̸� ��� �̺�Ʈ �߻� IsAlive�� �ڵ������� false�� ��ȯ
                if (m_CurrentHealth <= 0.0f)
                {
                    onDie?.Invoke();
                }
            }
        }
    }

    public Action onDie = null;

    public event Action<float> onHealthChange = null;

    private void Awake()
    {
        inverseMaxHealth = 1 / maxHealth;
    }

    public void OnDamage(float damage) 
    {
        CurrentHealth -= damage;
    }

    public void OnHeal(float amount)
    {
        CurrentHealth += amount;
    }

    public void Initialize()
    {
        m_CurrentHealth = maxHealth;
        onHealthChange?.Invoke(1.0f);
    }
}
