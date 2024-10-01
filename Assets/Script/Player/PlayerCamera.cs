using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform gunPoint;

    public Transform ads;

    public Transform gunMesh;

    Vector3 currentGunPosition;

    bool isAim = false;

    CinemachineVirtualCamera vcam;
    CinemachinePOV pov;

    Transform cm;

    private void Awake()
    {
        cm = transform.GetChild(0);
        currentGunPosition = gunPoint.localPosition;

        vcam = GetComponent<CinemachineVirtualCamera>();
        pov = vcam.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Update()
    {
        //gunPoint.localPosition = Quaternion.Euler(pov.m_VerticalAxis.Value, pov.m_HorizontalAxis.Value, 0) * currentGunPosition;
        gunPoint.localEulerAngles = Vector3.right * pov.m_VerticalAxis.Value + Vector3.up * pov.m_HorizontalAxis.Value;
    }

    public void Aim()
    {
        isAim = true;
        ads.localPosition = Quaternion.Euler(pov.m_VerticalAxis.Value, pov.m_HorizontalAxis.Value, 0) * ads.localPosition;
        ads.localEulerAngles = Vector3.right * pov.m_VerticalAxis.Value + Vector3.up * pov.m_HorizontalAxis.Value;
        gunMesh.parent = ads;
        currentGunPosition = ads.localPosition;
    }
}
