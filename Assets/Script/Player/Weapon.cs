using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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

    PlayerMovementContoller player;

    Light m_FireLight;

    float lightTime = 0.1f;

    // �Ѿ��� �� ����
    int m_TotalAmmo;

    // ���� �Ѿ��� ����
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

        if(m_CurrentAmmo < 1 && m_TotalAmmo > 0) // �Ѿ��� �� ������ 0���ϸ� ������ �Ұ���
        {
            // ������ �˸�
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        // ���� ������ �˸��� ��������Ʈ ����
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

        // ���� ���� �˸��� ��������Ʈ ����
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
