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

        // �⺻ ����� ���⸦ ��� ���� �ʱ� ������ hand ���̾��� weight �� 0 ���� �����Ͽ� ���� ���ش�.
        // layer weight�� ������Ʈ�� Ȱ��ȭ�� ������ �ʱ�ȭ �Ǳ⶧���� Ȱ��ȭ �� ������ 0���� �������ش�.
        m_Animator.SetLayerWeight(hand_LayerNum, 0f);
    }
}
