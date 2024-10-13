using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public GameObject[] m_Weapons;

    Transform m_PlayerWeapons;

    Transform m_WeaponPoint;

    Player m_Player;

    const int Pistol = 0;
    const int Rifle = 1;
    const int Shotgun = 2;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_PlayerWeapons = child.GetChild(0);

        child = transform.GetChild(2);
        m_PlayerWeapons = child;

        foreach(var weapon in m_Weapons)
        {
            GameObject obj = Instantiate(weapon);
            obj.transform.parent = m_PlayerWeapons;
            obj.SetActive(false);
        }
    }

    private void Start()
    {
        m_Player = GameManager.Instance.Player;

        m_Player.onKeyOne += OnKeyOne;
        m_Player.onKeyTwo += OnKeyTwo;
        m_Player.onKeyThree += OnKeyThree;

        m_Weapons[0].GetComponent<Weapon>().Activate = true;

        SetWeapon(Pistol);
    }

    private bool HasWeapon(int index)
    {
        return m_Weapons[index].GetComponent<Weapon>().Activate;
    }

    private void SetWeapon(int index)
    {
        if (HasWeapon(index))
        {
            m_Weapons[index].transform.parent = m_WeaponPoint;
            m_Weapons[index].SetActive(true);

            m_Player.SetWeaponSetting(m_Weapons[index].GetComponent<Weapon>());
        }
    }

    public void AddWeapon(int index)
    {
        if (!HasWeapon(index))
        {
            m_Weapons[index].GetComponent<Weapon>().Activate = true;
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

