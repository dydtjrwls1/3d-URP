using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0.5f;
    public int maxAmmo = 12;

    int m_CurrentAmmo;

    private void Awake()
    {
        m_CurrentAmmo = maxAmmo;
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;

        player.onBulletFire += OnBulletFired;
    }

    private void OnBulletFired()
    {
        m_CurrentAmmo--;
    }


}
