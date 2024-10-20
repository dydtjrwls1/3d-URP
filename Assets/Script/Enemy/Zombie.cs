using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyBase
{
    int hand_LayerNum;

    protected override void Awake()
    {
        base.Awake();
        hand_LayerNum = m_Animator.GetLayerIndex("Hand");
    }

    protected override void OnReset()
    {
        base.OnReset();

        // 기본 좀비는 무기를 들고 있지 않기 때문에 hand 레이어의 weight 를 0 으로 설정하여 손을 펴준다.
        // layer weight는 오브젝트가 활성화될 때마다 초기화 되기때문에 활성화 될 때마다 0으로 설정해준다.
        m_Animator.SetLayerWeight(hand_LayerNum, 0f);
    }
}
