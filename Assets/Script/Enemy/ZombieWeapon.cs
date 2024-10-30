using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWeapon : Zombie
{
    public Transform weaponPivot;

    EnemyWeapon m_CurrentWeapon;

    int hand_LayerNum;

    protected override void Awake()
    {
        base.Awake();
        hand_LayerNum = m_Animator.GetLayerIndex("Hand");
    }

    // objectpool ���� Instantiate �� �� �ѹ��� �����Ѵ�.
    public override void Equip()
    {
        base.Equip();
        m_CurrentWeapon = Factory.Instance.GetRandomEnemyWeapon(weaponPivot.position);
        m_CurrentWeapon.transform.rotation = weaponPivot.rotation;
        m_CurrentWeapon.transform.SetParent(weaponPivot);
    }

    // objectpool ���� pull �� ������ ����ȴ�.
    protected override void OnReset()
    {
        base.OnReset();
        // ���⸦ �� ���� ǥ���ϱ� ���� �ƹ�Ÿ ����ũ�� ����� �ִϸ������� ���̾��� weight�� 1�� �����Ѵ�. (�����ø��� �ʱ�ȭ �Ǳ� ������ reset �� ������ �������ش�.)
        m_Animator.SetLayerWeight(hand_LayerNum, 1f);

        // ���� ����� enemybase�� ������ �ʱ� ������ ���� activate ���ش�.
        m_CurrentWeapon?.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_CurrentWeapon?.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        //if (m_CurrentWeapon != null)
        //{
        //    m_CurrentWeapon.transform.position = weaponPivot.position;
        //    m_CurrentWeapon.transform.rotation = weaponPivot.rotation;
        //}
    }
}