using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpItem : RecycleObject
{
    ItemData data;

    public ItemData Data
    {
        get => data;
        set
        {
            // �ѹ� �������� �ٲ��� �ʴ´�.
            if (data == null)
            {
                data = value;
                Instantiate(data.prefab, m_Pivot); // pivot ����Ʈ�� ������ �Ž� ����
            }
            else
            {
                Debug.Log("�̹� �����Ͱ� ������ �����ۿ� �����͸� �ٲٷ��� �õ��߽��ϴ�.");
            }
        }
    }

    // ���� ��� Ʈ������
    Transform m_Pivot;

    private void Awake()
    {
        m_Pivot = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // pickup������ ����ΰ��
        IPickUp pickUp = other.GetComponent<IPickUp>();
        if (pickUp != null)
        {
            // pickUP�Լ� ����
            pickUp.PickUp(data.code, data.capacity);
            DisableTimer(); // ��Ȱ��ȭ
        }
    }
}
