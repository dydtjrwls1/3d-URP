using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : ObjectPool<PickUpItem>
{
    public ItemData data;

    protected override void OnGenerateObject(PickUpItem comp)
    {
        comp.Data = data;
    }
}
