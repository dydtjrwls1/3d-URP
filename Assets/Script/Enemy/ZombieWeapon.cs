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

    // objectpool 에서 Instantiate 될 때 한번만 실행한다.
    public override void Equip()
    {
        base.Equip();
        m_CurrentWeapon = Factory.Instance.GetRandomEnemyWeapon(weaponPivot.position);
        m_CurrentWeapon.transform.rotation = weaponPivot.rotation;
        m_CurrentWeapon.transform.SetParent(weaponPivot);
    }

    // objectpool 에서 pull 될 때마다 실행된다.
    protected override void OnReset()
    {
        base.OnReset();
        // 무기를 쥔 손을 표현하기 위해 아바타 마스크가 적용된 애니메이터의 레이어의 weight를 1로 설정한다. (생성시마다 초기화 되기 때문에 reset 될 때마다 수정해준다.)
        m_Animator.SetLayerWeight(hand_LayerNum, 1f);

        // 현재 무기는 enemybase에 속하지 않기 때문에 따로 activate 해준다.
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