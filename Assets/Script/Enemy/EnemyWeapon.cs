using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : RecycleObject
{
    Transform orgParent;

    private void Awake()
    {
        orgParent = transform.parent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (transform.parent.gameObject.activeInHierarchy)
        {
            transform.parent = orgParent;
        }
        
    }
}
