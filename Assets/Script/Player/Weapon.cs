using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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

    PlayerMovementContoller player;

    Light m_FireLight;

    float lightTime = 0.1f;

    // 총알의 총 개수
    int m_TotalAmmo;

    // 현재 총알의 개수
    int m_CurrentAmmo;

    bool m_IsActivate = false;

    public GameObject PrefabObject { get; set; }



    public bool Activate
    {
        get => m_IsActivate;
        set => m_IsActivate = value;
    }

    public Action<float> onReloadTimeChange = null;

    //public Action onReloadStart = null;

    //public Action<int> onReloadEnd = null;

    public Action<bool> onReload = null;

    public event Action<int> onCurrentBulletChange = null;

    public event Action<int> onTotalBulletChange = null;

    public int CurrentAmmo
    {
        get => m_CurrentAmmo;
        set
        {
            if (m_CurrentAmmo != value)
            {
                m_CurrentAmmo = value;

                onCurrentBulletChange?.Invoke(m_CurrentAmmo);
            }
        }
    }

    public Light FireLight => m_FireLight;

    public int TotalAmmo
    {
        get => m_TotalAmmo;
        set
        {
            if (m_TotalAmmo != value)
            {
                m_TotalAmmo = Mathf.Max(value, 0);
                onTotalBulletChange?.Invoke(m_TotalAmmo);
            }
        }
    }

    public bool CanFire => m_CurrentAmmo > 0;

    

    private void Awake()
    {
        player = GameManager.Instance.Player;
        m_FireLight = GetComponentInChildren<Light>();

        m_CurrentAmmo = maxAmmo;
        m_TotalAmmo = maxAmmo * 5;
    }

    private void OnEnable()
    {
        player.onBulletFire += OnBulletFired;
    }

    private void OnDisable()
    {
        player.onBulletFire -= OnBulletFired;
    }


    private void OnBulletFired(Weapon _)
    {
        CurrentAmmo -= 1;

        if(m_CurrentAmmo < 1 && m_TotalAmmo > 0) // 총알의 총 개수가 0이하면 재장전 불가능
        {
            // 재장전 알림
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        // 장전 시작을 알리는 델리게이트 실행
        onReload?.Invoke(true);

        float elapsedTime = 0.0f;
        float inverseReloadTime = 1 / reloadTime;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;

            onReloadTimeChange?.Invoke(elapsedTime * inverseReloadTime);

            yield return null;  
        }

        TotalAmmo -= (maxAmmo - CurrentAmmo);
        CurrentAmmo = maxAmmo;

        // 장전 끝을 알리는 델리게이트 실행
        onReload?.Invoke(false);
    }

    public void StartFireEffectCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine(OnFireEffect());
    }

    IEnumerator OnFireEffect()
    {
        float elapsedTime = lightTime;

        m_FireLight.enabled = true;
        fireEffect.Play();

        while (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        m_FireLight.enabled = false;

        //m_FireLightOnCoroutune = null;
    }
}
