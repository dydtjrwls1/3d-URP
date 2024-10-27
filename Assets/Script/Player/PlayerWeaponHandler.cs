using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour, IPickUp
{
    public GameObject[] m_WeaponPrefabs;

    public Transform m_WeaponPoint;

    Player m_Player;

    GameObject m_CurrentWeaponPrefab;

    bool m_IsReload = false;

    

    

    const int Pistol = (int)ItemCode.Pistol;
    const int Rifle = (int)ItemCode.Rifle;
    const int Shotgun = (int)ItemCode.Shotgun;

    

    private void Awake()
    {
        foreach(var prefab in m_WeaponPrefabs)
        {
            GameObject obj = Instantiate(prefab, m_WeaponPoint);

            Weapon w = obj.GetComponent<Weapon>();

            // ���� �߿��� ���� �ٲ��� ���ϰ� �ϱ� ���� ��ġ
            w.onReload += (isReload) => { m_IsReload = isReload; }; // ���� ���� �� isreload �� false �� �Ǹ鼭 ���� �ٲ� �� ��������.

            w.PrefabObject = obj; 
            obj.SetActive(false);
        }
    }

    private void Start()
    {
        m_Player = GameManager.Instance.Player;

        m_Player.onKeyOne += OnKeyOne;
        m_Player.onKeyTwo += OnKeyTwo;
        m_Player.onKeyThree += OnKeyThree;

        AddWeapon(Pistol);
        AddWeapon(Rifle);

        SetWeapon(Pistol);
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

                m_Player.SetWeaponSetting(weapon);
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
}

