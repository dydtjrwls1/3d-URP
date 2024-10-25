using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPoint_Base : MonoBehaviour
{
    [SerializeField]
    protected float spawnInterval = 5.0f;

    [SerializeField]
    Texture2D intervalImage;

    // 게임 내에 보여질 이미지
    Image m_CoolDownImage;

    protected bool m_CanSpawn = true;

    float m_CurrentIntervalTime;
    float m_InverseInterval; // 나누기로 인한 성능저하를 방지하기 위한 변수

    float CurrentIntervalTime
    {
        get => m_CurrentIntervalTime;
        set
        {
            if(m_CurrentIntervalTime != value)
            {
                m_CurrentIntervalTime = value;
                m_CoolDownImage.fillAmount = m_CurrentIntervalTime * m_InverseInterval;
            }
        }
    }

    private void Awake()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        Transform child = canvas.transform.GetChild(0);

        m_CoolDownImage = child.GetComponent<Image>();

        Material material = m_CoolDownImage.material;
        material.SetTexture("_BaseMap", intervalImage);
    }

    private void Start()
    {
        m_InverseInterval = 1 / spawnInterval;

        CurrentIntervalTime = 0.0f;
    }

    private void Update()
    {
        CurrentIntervalTime += Time.deltaTime;

        if (CurrentIntervalTime > spawnInterval)
        {
            CurrentIntervalTime = 0.0f;
            Spawn();
        }
    }

    protected virtual void Spawn() { }
}
