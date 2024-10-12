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

    public bool CanFire => m_CurrentAmmo > 0;

    public event Action<int> onBulletChange = null;

    private void Awake()
    {
        m_CurrentAmmo = maxAmmo;
        m_FireLight = GetComponentInChildren<Light>();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        Player player = GameManager.Instance.Player;

        player.onBulletFire += OnBulletFired;
    }

    private void OnDisable()
    {
        Player player = GameManager.Instance.Player;

        player.onBulletFire -= OnBulletFired;
    }

    private void OnBulletFired(Weapon _)
    {
        m_CurrentAmmo--;

        onBulletChange?.Invoke(m_CurrentAmmo);

        if(m_CurrentAmmo < 1)
        {
            // ������ �˸�
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;

            yield return null;  
        }

        m_CurrentAmmo = maxAmmo;
    }
}
