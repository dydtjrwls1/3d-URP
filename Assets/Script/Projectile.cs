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
        // ������ ��ġ���� ������ ���� �������� ����
        m_Velocity = transform.forward * speed;

        // Ÿ�̸� ����
        DisableTimer(lifeTime);
    }

    private void FixedUpdate()
    {
        m_Rb.MovePosition(m_Rb.position + Time.fixedDeltaTime * speed * m_Velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��� ��Ȱ��ȭ
        DisableTimer(0.0f);
    }
}
