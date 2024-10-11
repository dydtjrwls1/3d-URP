using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Basic")]
    // 발사 속도
    public float fireRate = 0.5f;

    // 반동 세기
    public float recoilAmount = 1.0f;

    // 에임 시 반동 세기
    public float aimRecoilAmount = 0.1f;

    // 탄창량
    public int maxAmmo = 12;

    // 카메라 위치에 맞게 메쉬의 위치조정을 위한 값
    public Vector3 offset = Vector3.zero;

    [Header("Effect")]
    // 발사 위치
    public Transform firePoint;

    // 발사 효과
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
            // 재장전 알림
            m_CurrentAmmo = maxAmmo;
        }
    }


}
