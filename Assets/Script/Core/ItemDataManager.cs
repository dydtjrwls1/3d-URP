using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    public ItemData[] itemDatas;

    // ÀÎµ¦»ç
    public ItemData this[ItemCode code] => itemDatas[(int)code];

    public ItemData this[int index] => itemDatas[index];
}
