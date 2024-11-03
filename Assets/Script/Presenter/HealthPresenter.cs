using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPresenter : MonoBehaviour
{
    [SerializeField]
    Health health;

    [SerializeField]
    Slider slider;

    private void Start()
    {
        health.onHealthChange += OnHealthChange;
    }

    private void OnDisable()
    {
        health.onHealthChange -= OnHealthChange;
    }

    public void OnDamage(float damage)
    {
        health.OnDamage(damage);
    }

    public void OnHeal(float heal)
    {
        health.OnHeal(heal);
    }

    private void OnHealthChange(float ratio)
    {
        // health 에 변화가 있을때마다 실행
        if (slider != null)
        {
            slider.value = Mathf.Clamp01(ratio);
        }
    }
}
