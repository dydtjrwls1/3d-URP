using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthGauge : MonoBehaviour
{
    Slider healthSlider;

    private void Awake()
    {
        healthSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        Health health = GameManager.Instance.Player.Health;
        health.onHealthChange += (ratio) => { healthSlider.value = ratio; };
    }
}
