using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyBase, IEquipable
{
    public Transform equipPivot;

    public Transform EquipPivot { get => equipPivot; }

    public EnemyEquipment Equipment { get; private set; }

    public void Equip()
    {
        // 50 % Ȯ���� ����� ���� ���´�.
        bool isEquip = Random.value > 0.5f ? true : false;
        if (isEquip)
        {
            Equipment = Factory.Instance.GetRandomEnemyEquipment();
        }
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // ������ ���� ����
        Equipment?.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if(Equipment != null)
        {
            Equipment.transform.position = EquipPivot.position;
            Equipment.transform.rotation = EquipPivot.rotation;
        }
    }
}
