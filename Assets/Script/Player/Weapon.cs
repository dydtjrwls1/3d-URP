using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // �߻� �ӵ�
    public float fireRate = 0.5f;

    // źâ��
    public int maxAmmo = 12;

    // ī�޶� ��ġ�� �°� �޽��� ��ġ������ ���� ��
    public Vector3 offset = Vector3.zero;

    // �߻� ��ġ
    public Transform firePoint;

    // �߻� ȿ��
    public ParticleSystem fireEffect;

    Light m_FireLight;

    int m_CurrentAmmo;

    public Light FireLight => m_FireLight;

    private void Awake()
    {
        m_CurrentAmmo = maxAmmo;
        m_FireLight = GetComponentInChildren<Light>();
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;

        player.onBulletFire += OnBulletFired;
    }

    private void OnBulletFired()
    {
        m_CurrentAmmo--;
    }


}
