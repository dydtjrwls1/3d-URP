using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrenadeHandler : MonoBehaviour
{
    // 1��Ī ȭ�鿡 ������ ��ź ������
    public GameObject grenadePrefab;

    int m_GrenadeCount = 3;     // �⺻������ ������ �ִ� ��ź�� ��
    int m_MaxGrenadeCount = 5;  // ���� �� �ִ� ��ź�� �ѷ�

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

        // ��ô �غ����¿��� ���⺯�� �� ��ô�غ� ���� ������ ���� ����
        PlayerMovementContoller playerMovementContoller = GetComponent<PlayerMovementContoller>();
        //playerMovementContoller.onWeaponChange += (_) =>
        //{
        //    GrenadeDeactivate();
        //};
    }

    // ��ô �غ���¿� ���Խ� ������ �Լ�
    private void GrenadeReady()
    {
        if(GrenadeCount > 0)
        {
            m_IsGrenadeReady = true;
            m_GrenadeGameObject?.SetActive(true);
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }

    // ��ô�غ���¿��� ���콺 ����Ŭ������ �߻����� �� ����Ǵ� �Լ�
    private void GrenadeFire(bool _)
    { 
        if (m_IsGrenadeReady)
        {
            m_GrenadeGameObject?.SetActive(false);

            // ��ô���� �߻�
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
