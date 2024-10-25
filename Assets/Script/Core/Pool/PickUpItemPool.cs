using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemPool : ObjectPool<PickUpItem>
{
    public ItemStack[] itemStacks;

    // ������ �Ⱦ� �������� �ڵ�
    //public ItemCode itemCode;

    // ������ �������� ������
    //ItemData data;

    ItemStack CurrentStack
    {
        get
        {
            // ���ÿ��� �ϳ� ���������� Count ����
            m_CurrentCount++;

            // Count�� ������ size ���� Ŀ����� Count �ʱ�ȭ �� ���� ��������
            if (m_CurrentCount > itemStacks[m_Index].size)
            {
                m_CurrentCount = 0;
                m_Index = (m_Index + 1) % itemStacks.Length;
            }

            return itemStacks[m_Index];
        }
    }

    int m_CurrentCount = 0;
    int m_Index = 0;

    private void Awake()
    {
        int totalSize = 0;

        foreach (var item in itemStacks) { totalSize += item.size; } // stack �������� ������ poolsize���� Ŭ ��� ����� ���
            
        if (totalSize > poolSize)
        {
            Debug.LogWarning("ItemStack �� size�� ������ PoolSize���� Ů�ϴ�.");
        }
    }

    protected override void OnGenerateObject(PickUpItem comp)
    {
        // Ǯ�� ������ ������ ������ ����
        // comp.Data = data;
        comp.Data = GameManager.Instance.ItemDataManager[CurrentStack.code];
    }
}

[Serializable]
public class ItemStack
{
    public ItemCode code;
    public int size;
}


