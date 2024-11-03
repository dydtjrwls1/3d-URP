using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour, IPickUp, IInitialize
{
    public GameObject[] m_WeaponPrefabs;

    // weapon �� ��ġ�� Ʈ������
    public Transform m_WeaponPoint;

    // ��ô���� ������
    public GameObject grenadePrefab;

    [SerializeField]
    int maxGrenadeCount = 5;

    // ��ô���Ⱑ ��ġ�� Ʈ������
    Transform m_GrenadePoint;

    // ��ô���� ������Ʈ
    GameObject m_GrenadeGameObject;

    // ���� ������ ������Ʈ
    GameObject m_CurrentWeaponPrefab;

    // ���� ��ô���� ����
    int m_CurrentGrenadeCount = 3;

    // ���� ���� True �̸� ���� ���̴� (=> ���� ����, �� �߻� �Ұ�)
    bool m_IsReload = false;

    // ��ô���� �غ� ����
    bool m_IsGrenadeReady = false;

    const int Pistol = (int)ItemCode.Pistol;
    const int Rifle = (int)ItemCode.Rifle;
    const int Shotgun = (int)ItemCode.Shotgun;

    // ���� ���Ⱑ ����Ǿ����� �˸��� ��������Ʈ
    public event Action<Weapon> onWeaponChange = null;

    // ��ô���� ���� ������ �˸��� ��������Ʈ
    public event Action<int> onGrenadeCountChange = null;

    // ��ô���� �߻� �غ� ���¸� �˸��� ��������Ʈ
    public event Action<bool> onGrenadeReady = null;

    int CurrentGrenadeCount
    {
        get => m_CurrentGrenadeCount;
        set
        {
            if(value < 0)
            {
                m_CurrentGrenadeCount = Mathf.Clamp(value, 0, maxGrenadeCount);
                onGrenadeCountChange?.Invoke(m_CurrentGrenadeCount);
            }
        }
    }

    private void Awake()
    {
        // ���� ������Ʈ ���� �� ��Ȱ��ȭ�ϱ�
        foreach(var prefab in m_WeaponPrefabs)
        {
            GameObject obj = Instantiate(prefab, m_WeaponPoint);

            Weapon w = obj.GetComponent<Weapon>();

            // ���� �߿��� ���� �ٲ��� ���ϰ� �ϱ� ���� ��ġ
            w.onReload += (isReload) => { m_IsReload = isReload; }; // ���� ���� �� isreload �� false �� �Ǹ鼭 ���� �ٲ� �� ��������.

            w.PrefabObject = obj; 
            obj.SetActive(false);
        }

        // ��ô���Ⱑ ��ġ�� Ʈ������
        Transform child = transform.GetChild(1);
        m_GrenadePoint = child.GetChild(4);
                
        //m_Player = GetComponent<PlayerMovementContoller>();
    }

    private void Start()
    {
        // ��ô���� ������Ʈ ����
        m_GrenadeGameObject = Instantiate(grenadePrefab, m_GrenadePoint);
        m_GrenadeGameObject.SetActive(false);

        PlayerInputController m_PlayerInputController = GetComponent<PlayerInputController>();

        m_PlayerInputController.onKeyOne += OnKeyOne;
        m_PlayerInputController.onKeyTwo += OnKeyTwo;
        m_PlayerInputController.onKeyThree += OnKeyThree;

        
    }

    private Weapon GetWeapon(int index)
    {
        if(index > m_WeaponPrefabs.Length - 1)
        {
            return null;
        }

        GameObject weapon = m_WeaponPoint.GetChild(index).gameObject;
        return weapon.GetComponent<Weapon>();
    }

    private bool HasWeapon(int index, out Weapon weapon)
    {
        bool result = false;

        weapon = GetWeapon(index);

        // ���Ⱑ �����ϰ� ���Ⱑ Ȱ��ȭ �Ǿ� ������� true ��ȯ
        result = weapon != null && weapon.Activate;

        return result;
    }

    public void SetWeapon(int index)
    {
        if (HasWeapon(index, out Weapon weapon))
        {
            // weapon �� ���������� �ҷ��Դٸ� ���⸦ �����Ѵ�.
            if (weapon != null && !m_IsReload && weapon.PrefabObject != m_CurrentWeaponPrefab) // ������ ���� �ƴϰ� ���� ����ִ� ����� ���� ���� �� 
            {
                // ���� �������� ���� ����
                if (m_CurrentWeaponPrefab != null)
                {
                    m_CurrentWeaponPrefab.SetActive(false);
                }

                weapon.PrefabObject.SetActive(true);
                m_CurrentWeaponPrefab = weapon.PrefabObject;

                onWeaponChange?.Invoke(weapon);
                //m_Player.SetWeaponSetting(weapon);
            }
        }
    }

    public void AddWeapon(int index)
    {
        if (!HasWeapon(index, out Weapon weapon))
        {
            weapon.Activate = true;
        }
    }

    private void OnKeyOne()
    {
        SetWeapon(Pistol);
    }

    private void OnKeyTwo()
    {
        SetWeapon(Rifle);
    }

    private void OnKeyThree()
    {
        SetWeapon(Shotgun);
    }

    private void OnGrenadeReady()
    {
        if(CurrentGrenadeCount > 0)
        {
            // ��ô���� ������Ʈ Ȱ��ȭ
            m_GrenadeGameObject.SetActive(true);

            m_IsGrenadeReady = true;

            // ��ô���� �غ� �Ϸ� �˸�
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }

    // ��ô�غ���¿��� ���콺 ����Ŭ������ �߻����� �� ����Ǵ� �Լ�
    private void OnGrenadeFire(bool _)
    {
        if (m_IsGrenadeReady)
        {
            m_GrenadeGameObject?.SetActive(false);

            // ��ô���� �߻�
            Factory.Instance.GetProjectile(Camera.main.transform.position + Camera.main.transform.forward * 0.5f);

            CurrentGrenadeCount--;
            m_IsGrenadeReady = false;
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }


    public void PickUp(ItemCode code, int capacity)
    {
        int index = (int)code;

        if (HasWeapon(index, out Weapon weapon))
        {
            if(weapon != null)
            {
                // �Ѿ� �߰�
                weapon.TotalAmmo += capacity;
            }
        }
    }

    public void Initialize()
    {
        // �� �߰��ϱ� (���� �ʿ����� ����)
        AddWeapon(Pistol);
        AddWeapon(Rifle);

        // ���� ���� Pistol�� ����
        SetWeapon(Pistol);
    }
}

