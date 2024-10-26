using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint_Trigger : SpawnPoint_Base
{
    PickUpItem m_Item = null;

    protected override void Awake()
    {
        base.Awake();
        m_CanSpawn = true;
    }



    protected override void Spawn()
    {
        m_Item = Factory.Instance.GetPickUpItem(transform.position, ItemCode.Pistol);
        m_CanSpawn = false;
    }
}
