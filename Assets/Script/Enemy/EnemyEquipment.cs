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
        // ��ü ��Ȱ��ȭ �� ���� �θ�� ���ư��� (Factory ��)
        if (transform.parent.gameObject.activeInHierarchy)
        {
            transform.SetParent(orgParent, false);
        }
    }
}
