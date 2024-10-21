using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipable
{
    Transform EquipPivot { get; }

    EnemyEquipment Equipment { get; }

    public void Equip();
}
