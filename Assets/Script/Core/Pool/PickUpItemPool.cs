using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemPool : ObjectPool<PickUpItem>
{
    //public ItemStack[] itemStacks;

    //// 생성할 픽업 아이템의 코드
    ////public ItemCode itemCode;

    //// 생성할 아이템의 데이터
    ////ItemData data;

    //ItemStack CurrentStack
    //{
    //    get
    //    {
    //        // 스택에서 하나 꺼낼때마다 Count 증가
    //        m_CurrentCount++;

    //        // Count가 스택의 size 보다 커질경우 Count 초기화 후 다음 스택으로
    //        if (m_CurrentCount > itemStacks[m_Index].size)
    //        {
    //            m_CurrentCount = 0;
    //            m_Index = (m_Index + 1) % itemStacks.Length;
    //        }

    //        return itemStacks[m_Index];
    //    }
    //}

    public ItemCode code;

    protected override void OnGenerateObject(PickUpItem comp)
    {
        // 풀에 생성된 아이템 데이터 설정
        // comp.Data = data;
        comp.Data = GameManager.Instance.ItemDataManager[code];
    }

    //// PickUp 풀에만 있는 코드를 받아서 특정 아이템 오브젝트를 반환하는 함수
    //public PickUpItem GetPickUpItem(ItemCode code ,Vector3? position = null, Vector3? eulerAngle = null)
    //{
    //    if (readyQueue.Count > 0)
    //    {
    //        // 아직 비활성화된 오브젝트가 남아있다.
    //        PickUpItem comp = readyQueue.Dequeue();                                                  //
    //        comp.transform.position = position.GetValueOrDefault();                         //
    //        comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());     //
    //        comp.gameObject.SetActive(true);                                                //

    //        return comp;
    //    }
    //    else
    //    {
    //        // 모든 오브젝트가 활성화되어 있다. => 남아있는 오브젝트가 없다.
    //        ExpandPool();
    //        return GetObject(position, eulerAngle);
    //    }
    //}
}

//[Serializable]
//public class ItemStack
//{
//    public ItemCode code;
//    public int size;
//}


