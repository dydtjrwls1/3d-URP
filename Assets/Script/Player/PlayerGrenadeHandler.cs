using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrenadeHandler : MonoBehaviour
{
    // 1ÀÎÄª È­¸é¿¡ º¸¿©Áú ÆøÅº ÇÁ¸®Æé
    public GameObject grenadePrefab;

    int m_GrenadeCount = 3;     // ±âº»ÀûÀ¸·Î °¡Áö°í ÀÖ´Â ÆøÅºÀÇ ¼ö
    int m_MaxGrenadeCount = 5;  // °¡Áú ¼ö ÀÖ´Â ÆøÅºÀÇ ÃÑ·®

    bool m_IsGrenadeReady = false;

    public bool IsGrenadeReady => m_IsGrenadeReady;

    PlayerMovementContoller player;

    Transform m_GrenadePoint;

    GameObject m_GrenadeGameObject;

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
        player = GetComponent<PlayerMovementContoller>();

        Transform child = transform.GetChild(1);

        m_GrenadePoint = child.GetChild(4);
    }

    private void Start()
    {
        m_GrenadeGameObject = Instantiate(grenadePrefab, m_GrenadePoint);
        m_GrenadeGameObject.SetActive(false);

        player.onGrenade += GrenadeReady;
        player.onGrenadeFire += GrenadeFire;
    }

    private void GrenadeReady()
    {
        if(GrenadeCount > 0)
        {
            m_IsGrenadeReady = true;
            m_GrenadeGameObject?.SetActive(true);
        }
    }

    private void GrenadeFire()
    { 
        if (m_IsGrenadeReady)
        {
            m_GrenadeGameObject?.SetActive(false);

            // ÅõÃ´¹«±â ¹ß»ç
            Factory.Instance.GetProjectile(Camera.main.transform.position);

            GrenadeCount--;
            m_IsGrenadeReady = false;
        }
    }
}
