using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : RecycleObject
{
    [SerializeField]
    float force = 5.0f;

    [SerializeField]
    float duration = 2.0f;

    Rigidbody rb;

    float elapsedTime = 0.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnReset()
    {
        rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);
        elapsedTime = 0.0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > duration)
        {
            Factory.Instance.GetExplosionEffect(transform.position);
            gameObject.SetActive(false);
        }
    }
}
