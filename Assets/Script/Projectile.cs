using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;

    Vector3 m_Velocity;

    void Start()
    {
        m_Velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(m_Velocity * Time.deltaTime);
    }
}
