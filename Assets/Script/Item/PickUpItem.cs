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
            // 한번 정해지면 바뀌지 않는다.
            if (data == null)
            {
                data = value;
                Instantiate(data.prefab, m_Pivot); // pivot 포인트에 아이템 매시 생성
            }
            else
            {
                Debug.Log("이미 데이터가 정해진 아이템에 데이터를 바꾸려고 시도했습니다.");
            }
        }
    }

    // 모델이 담길 트랜스폼
    Transform m_Pivot;

    private void Awake()
    {
        m_Pivot = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // pickup가능한 대상인경우
        IPickUp pickUp = other.GetComponent<IPickUp>();
        if (pickUp != null)
        {
            // pickUP함수 실행
            pickUp.PickUp(data.code, data.capacity);
            DisableTimer(); // 비활성화
        }
    }
}
