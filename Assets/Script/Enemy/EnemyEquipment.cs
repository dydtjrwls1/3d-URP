using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipment : RecycleObject
{
    Transform orgParent;

    private void Awake()
    {
        orgParent = transform.parent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // 객체 비활성화 시 원래 부모로 돌아가기 (Factory 쪽)
        if (transform.parent.gameObject.activeInHierarchy)
        {
            transform.SetParent(orgParent, false);
        }
    }
}
