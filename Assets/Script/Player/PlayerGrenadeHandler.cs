using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrenadeHandler : MonoBehaviour
{
    // 1인칭 화면에 보여질 폭탄 프리펩
    public GameObject grenadePrefab;

    int m_GrenadeCount = 3;     // 기본적으로 가지고 있는 폭탄의 수
    int m_MaxGrenadeCount = 5;  // 가질 수 있는 폭탄의 총량

    bool m_IsGrenadeReady = false;

    public bool IsGrenadeReady => m_IsGrenadeReady;

    PlayerInputController m_PlayerInputController;

    Transform m_GrenadePoint;

    GameObject m_GrenadeGameObject;

    public event Action<bool> onGrenadeReady = null;

    public int GrenadeCount
    {
        get => m_GrenadeCount;
        set
        {
            if (m_GrenadeCount != value)
            {
                m_GrenadeCount = Mathf.Clamp(value, 0, m_MaxGrenadeCount);
                onGrenadeCountChange?.Invoke(m_GrenadeCount);
            }
        }
    }

    public event Action<int> onGrenadeCountChange = null;

    private void Awake()
    {
        m_PlayerInputController = GetComponent<PlayerInputController>();

        Transform child = transform.GetChild(1);

        m_GrenadePoint = child.GetChild(4);
    }

    private void Start()
    {
        m_GrenadeGameObject = Instantiate(grenadePrefab, m_GrenadePoint);
        m_GrenadeGameObject.SetActive(false);

        m_PlayerInputController.onGrenade += GrenadeReady;
        m_PlayerInputController.onFire += GrenadeFire;

        // 투척 준비항태에서 무기변경 시 투척준비 상태 해제를 위한 연결
        PlayerMovementContoller playerMovementContoller = GetComponent<PlayerMovementContoller>();
        //playerMovementContoller.onWeaponChange += (_) =>
        //{
        //    GrenadeDeactivate();
        //};
    }

    // 투척 준비상태에 진입시 실행할 함수
    private void GrenadeReady()
    {
        if(GrenadeCount > 0)
        {
            m_IsGrenadeReady = true;
            m_GrenadeGameObject?.SetActive(true);
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }

    // 투척준비상태에서 마우스 왼쪽클릭으로 발사했을 시 실행되는 함수
    private void GrenadeFire(bool _)
    { 
        if (m_IsGrenadeReady)
        {
            m_GrenadeGameObject?.SetActive(false);

            // 투척무기 발사
            Factory.Instance.GetProjectile(Camera.main.transform.position + Camera.main.transform.forward * 0.5f);

            GrenadeCount--;
            m_IsGrenadeReady = false;
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }

    private void GrenadeDeactivate()
    {
        m_IsGrenadeReady = false;
        m_GrenadeGameObject?.SetActive(false);
        onGrenadeReady?.Invoke(m_IsGrenadeReady);
    }
}
