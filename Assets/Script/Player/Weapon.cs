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


}
