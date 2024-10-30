using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyBase, IEquipable
{
    public Transform equipPivot;

    public Transform EquipPivot { get => equipPivot; }

    public EnemyEquipment Equipment { get; private set; }

    // ���� ���� �� ������Ʈ Ǯ���� Instantiate �� �� �ѹ��� ����ȴ�.
    public virtual void Equip()
    {
        // 50 % Ȯ���� ����� ���� ���´�.
        bool isEquip = Random.value > 0.5f ? true : false;
        if (isEquip)
        {
            Equipment = Factory.Instance.GetRandomEnemyEquipment(equipPivot.position);
            Equipment.transform.rotation = equipPivot.rotation;
            Equipment.transform.SetParent(equipPivot);
            // Equipment.transform.localScale = Vector3.one;
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        Equipment?.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // ������ ���� ����
        Equipment?.gameObject.SetActive(false);
    }

    //protected override void Update()
    //{
    //    base.Update();
    //    if(Equipment != null)
    //    {
    //        Equipment.transform.position = EquipPivot.position;
    //        Equipment.transform.rotation = EquipPivot.rotation;
    //    }
    //}
}
