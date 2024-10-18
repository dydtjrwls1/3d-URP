using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Basic")]
    // �߻� �ӵ�
    public float fireRate = 0.5f;

    public float reloadTime = 0.5f;

    // �ݵ� ����
    public float recoilAmount = 1.0f;

    // �ݵ� �ð�
    public float recoilTime = 0.1f;

    // ���� �� �ݵ� ����
    public float aimRecoilAmount = 0.1f;

    // źâ��
    public int maxAmmo = 12;

    // �⺻ ������
    public float defaultDamage = 1;

    // ī�޶� ��ġ�� �°� �޽��� ��ġ������ ���� ��
    public Vector3 offset = Vector3.zero;

    [Header("Effect")]
    // �߻� ��ġ
    public Transform firePoint;

    // �߻� ȿ��
    public ParticleSystem fireEffect;

    Light m_FireLight;

    int m_CurrentAmmo;

    bool m_IsActivate = false;

    public GameObject PrefabObject { get; set; }

    public bool Activate
    {
        get => m_IsActivate;
        set => m_IsActivate = value;
    }

    public Action<float> onReloadTimeChange = null;

    public int CurrentAmmo
    {
        get => m_CurrentAmmo;
        set
        {
            if (m_CurrentAmmo != value)
            {
                m_CurrentAmmo = value;

                onBulletChange?.Invoke(m_CurrentAmmo);
            }
        }
    }

    public Light FireLight => m_FireLight;

    public bool CanFire => m_CurrentAmmo > 0;

    public event Action<int> onBulletChange = null;

    private void Awake()
    {
        CurrentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        Player player = GameManager.Instance.Player;

        m_FireLight = GetComponentInChildren<Light>();

        player.onBulletFire += OnBulletFired;
    }

    private void OnDisable()
    {
        Player player = GameManager.Instance.Player;

        player.onBulletFire -= OnBulletFired;
    }

    private void OnBulletFired(Weapon _)
    {
        CurrentAmmo -= 1;

        if(m_CurrentAmmo < 1)
        {
            // ������ �˸�
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        float elapsedTime = 0.0f;
        float inverseReloadTime = 1 / reloadTime;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;

            onReloadTimeChange?.Invoke(elapsedTime * inverseReloadTime);

            yield return null;  
        }

        CurrentAmmo = maxAmmo;
    }
}
