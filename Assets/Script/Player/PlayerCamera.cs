using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform gunPoint;

    Vector3 originGunLocalPosition;

    CinemachineVirtualCamera vcam;
    CinemachinePOV pov;

    private void Awake()
    {
        originGunLocalPosition = gunPoint.localPosition;

        vcam = GetComponent<CinemachineVirtualCamera>();
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Update()
    {
        gunPoint.localPosition = Quaternion.Euler(pov.m_VerticalAxis.Value, pov.m_HorizontalAxis.Value, 0) * originGunLocalPosition;
        gunPoint.localEulerAngles = Vector3.right * pov.m_VerticalAxis.Value + Vector3.up * pov.m_HorizontalAxis.Value;
    }
}
