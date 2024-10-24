using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public GameObject[] m_WeaponPrefabs;

    public Transform m_WeaponPoint;

    Player m_Player;

    GameObject m_CurrentWeaponPrefab;

    bool m_OnReload = false;

    const int Pistol = 0;
    const int Rifle = 1;
    const int Shotgun = 2;

    private void Awake()
    {
        foreach(var prefab in m_WeaponPrefabs)
        {
            GameObject obj = Instantiate(prefab, m_WeaponPoint);
            Weapon w = obj.GetComponent<Weapon>();
            w.onReload += (isReload) => { m_OnReload = isReload; };
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

    private bool HasWeapon(int index)
    {
        Weapon weapon = GetWeapon(index);

        if(weapon == null)
        {
            return false;
        }
        else
        {
            return weapon.Activate;
        }
    }

    public void SetWeapon(int index)
    {
        if (HasWeapon(index))
        {
            Weapon weapon = GetWeapon(index);

            // weapon 을 정상적으로 불러왔다면 무기를 장착한다.
            if (weapon != null && !m_OnReload) // 재장전 중이 아닐 때 
            {
                // 현재 착용중인 무기 해제
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
        if (!HasWeapon(index))
        {
            Weapon weapon = GetWeapon(index);

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
}

