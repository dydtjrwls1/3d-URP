using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWeapon : EnemyBase
{
    public Transform weaponPivot;

    EnemyWeapon m_CurrentWeapon;

    protected override void OnReset()
    {
        base.OnReset();

        m_CurrentWeapon = Factory.Instance.GetRandomEnemyWeapon(weaponPivot.position);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_CurrentWeapon.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (m_CurrentWeapon != null)
        {
            m_CurrentWeapon.transform.position = weaponPivot.position;
            m_CurrentWeapon.transform.rotation = weaponPivot.rotation;
        }
    }
}