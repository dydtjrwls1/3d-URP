using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Basic")]
    // �߻� �ӵ�
    public float fireRate = 0.5f;

    // �ݵ� ����
    public float recoilAmount = 1.0f;

    // ���� �� �ݵ� ����
    public float aimRecoilAmount = 0.1f;

    // źâ��
    public int maxAmmo = 12;

    // ī�޶� ��ġ�� �°� �޽��� ��ġ������ ���� ��
    public Vector3 offset = Vector3.zero;

    [Header("Effect")]
    // �߻� ��ġ
    public Transform firePoint;

    // �߻� ȿ��
    public ParticleSystem fireEffect;

    Light m_FireLight;

    int m_CurrentAmmo;

    public Light FireLight => m_FireLight;

    public event Action<int> onBulletChange = null;

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

    private void OnBulletFired(Weapon _)
    {
        m_CurrentAmmo--;
        onBulletChange?.Invoke(m_CurrentAmmo);

        if(m_CurrentAmmo < 0)
        {
            // ������ �˸�
            m_CurrentAmmo = maxAmmo;
        }
    }


}
