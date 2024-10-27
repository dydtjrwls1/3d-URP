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

            // 장전 중에는 총을 바꾸지 못하게 하기 위한 조치
            w.onReload += (isReload) => { m_IsReload = isReload; }; // 장전 중일 때 isreload 가 false 가 되면서 총을 바꿀 수 없어진다.

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

        // 무기가 존재하고 무기가 활성화 되어 있을경우 true 반환
        result = weapon != null && weapon.Activate;

        return result;
    }

    public void SetWeapon(int index)
    {
        if (HasWeapon(index, out Weapon weapon))
        {
            // weapon 을 정상적으로 불러왔다면 무기를 장착한다.
            if (weapon != null && !m_IsReload && weapon.PrefabObject != m_CurrentWeaponPrefab) // 재장전 중이 아니고 지금 들고있는 무기와 같지 않을 때 
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
                // 총알 추가
                weapon.TotalAmmo += capacity;
            }
        }
    }
}

