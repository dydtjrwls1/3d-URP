using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemPool : ObjectPool<PickUpItem>
{
    //public ItemStack[] itemStacks;

    //// ������ �Ⱦ� �������� �ڵ�
    ////public ItemCode itemCode;

    //// ������ �������� ������
    ////ItemData data;

    //ItemStack CurrentStack
    //{
    //    get
    //    {
    //        // ���ÿ��� �ϳ� ���������� Count ����
    //        m_CurrentCount++;

    //        // Count�� ������ size ���� Ŀ����� Count �ʱ�ȭ �� ���� ��������
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
        // Ǯ�� ������ ������ ������ ����
        // comp.Data = data;
        comp.Data = GameManager.Instance.ItemDataManager[code];
    }

    //// PickUp Ǯ���� �ִ� �ڵ带 �޾Ƽ� Ư�� ������ ������Ʈ�� ��ȯ�ϴ� �Լ�
    //public PickUpItem GetPickUpItem(ItemCode code ,Vector3? position = null, Vector3? eulerAngle = null)
    //{
    //    if (readyQueue.Count > 0)
    //    {
    //        // ���� ��Ȱ��ȭ�� ������Ʈ�� �����ִ�.
    //        PickUpItem comp = readyQueue.Dequeue();                                                  //
    //        comp.transform.position = position.GetValueOrDefault();                         //
    //        comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());     //
    //        comp.gameObject.SetActive(true);                                                //

    //        return comp;
    //    }
    //    else
    //    {
    //        // ��� ������Ʈ�� Ȱ��ȭ�Ǿ� �ִ�. => �����ִ� ������Ʈ�� ����.
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


