using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadeHandler : MonoBehaviour
{
    int m_GrenadeCount = 3;     // �⺻������ ������ �ִ� ��ź�� ��
    int m_MaxGrenadeCount = 5;  // ���� �� �ִ� ��ź�� �ѷ�

    public int GrenadeCount
    {
        get => m_GrenadeCount;
        set
        {
            if (m_GrenadeCount != value)
            {
                m_GrenadeCount = Mathf.Clamp(value, 0, m_MaxGrenadeCount);
                onGrenadeCountChange?.Invoke(m_GrenadeCount);
            }
        }
    }

    public event Action<int> onGrenadeCountChange = null;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.onGrenade += OnGrenadeKey;
    }

    private void OnGrenadeKey()
    {
        throw new NotImplementedException();
    }
}
