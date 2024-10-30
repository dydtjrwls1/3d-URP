using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyBase>
{
    protected override void OnGenerateObject(EnemyBase comp)
    {
        comp.Player = GameManager.Instance.Player;

        // ��� ������ �� �ִ� ���� ������ �� ��� �̸� �����ؼ� �ش� ���� transform �� �ű��.
        IEquipable equipable = comp.GetComponent<IEquipable>();

        if(equipable != null)
        {
            equipable.Equip();
        }
    }
}
