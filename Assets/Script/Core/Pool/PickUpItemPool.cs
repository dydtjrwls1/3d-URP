using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemPool : ObjectPool<PickUpItem>
{
    public ItemStack[] itemStacks;

    // 생성할 픽업 아이템의 코드
    //public ItemCode itemCode;

    // 생성할 아이템의 데이터
    //ItemData data;

    ItemStack CurrentStack
    {
        get
        {
            // 스택에서 하나 꺼낼때마다 Count 증가
            m_CurrentCount++;

            // Count가 스택의 size 보다 커질경우 Count 초기화 후 다음 스택으로
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

        foreach (var item in itemStacks) { totalSize += item.size; } // stack 사이즈의 총합이 poolsize보다 클 경우 경고문구 출력
            
        if (totalSize > poolSize)
        {
            Debug.LogWarning("ItemStack 의 size의 총합이 PoolSize보다 큽니다.");
        }
    }

    protected override void OnGenerateObject(PickUpItem comp)
    {
        // 풀에 생성된 아이템 데이터 설정
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


