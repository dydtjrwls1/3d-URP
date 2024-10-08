using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : RecycleObject
{
    public float speed = 10.0f;

    public float lifeTime = 5.0f;

    Rigidbody m_Rb;

    Vector3 m_Velocity;

    public Vector3 Velocity
    {
        get => m_Velocity;
        set => m_Velocity = value;
    }

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
    }

    protected override void OnReset()
    {
        // 타이머 설정
        DisableTimer(lifeTime);
    }

    private void FixedUpdate()
    {
        m_Rb.MovePosition(m_Rb.position + Time.fixedDeltaTime * speed * m_Velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 즉시 비활성화
        DisableTimer(0.0f);
    }
}
