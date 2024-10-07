using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : RecycleObject
{
    public float speed = 10.0f;

    public float lifeTime = 5.0f;

    Rigidbody m_Rb;

    Vector3 m_Velocity;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
    }

    protected override void OnReset()
    {
        // 생성된 위치에서 앞으로 가는 방향으로 설정
        m_Velocity = transform.forward * speed;

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
