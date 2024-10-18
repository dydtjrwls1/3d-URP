using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Basic")]
    // 발사 속도
    public float fireRate = 0.5f;

    public float reloadTime = 0.5f;

    // 반동 세기
    public float recoilAmount = 1.0f;

    // 반동 시간
    public float recoilTime = 0.1f;

    // 에임 시 반동 세기
    public float aimRecoilAmount = 0.1f;

    // 탄창량
    public int maxAmmo = 12;

    // 기본 데미지
    public float defaultDamage = 1;

    // 카메라 위치에 맞게 메쉬의 위치조정을 위한 값
    public Vector3 offset = Vector3.zero;

    [Header("Effect")]
    // 발사 위치
    public Transform firePoint;

    // 발사 효과
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
            // 재장전 알림
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
