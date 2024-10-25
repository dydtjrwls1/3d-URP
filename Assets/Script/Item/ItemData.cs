using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Objects/Item Data", order = 0)]
public class ItemData : ScriptableObject
{
    public ItemCode code;
    public GameObject prefab;
    public int capacity;
}
