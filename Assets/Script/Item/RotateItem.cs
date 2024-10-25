using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public float rotateSpeed = 180.0f;

    Transform pivot;

    private void Awake()
    {
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        pivot.Rotate(rotateSpeed * Time.deltaTime * Vector3.up);
    }
}
