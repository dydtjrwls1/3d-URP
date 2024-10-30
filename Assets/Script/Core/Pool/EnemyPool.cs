using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyBase>
{
    protected override void OnGenerateObject(EnemyBase comp)
    {
        comp.Player = GameManager.Instance.Player;

        // 장비를 장착할 수 있는 적은 생성될 때 장비를 미리 생성해서 해당 부위 transform 에 옮긴다.
        IEquipable equipable = comp.GetComponent<IEquipable>();

        if(equipable != null)
        {
            equipable.Equip();
        }
    }
}
