using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���� �ð��� ������� �ڵ����� �����ϴ� Ŭ����
public class SpawnPoint_Base : MonoBehaviour
{
    [SerializeField]
    protected float spawnInterval = 5.0f;

    [SerializeField]
    Texture2D intervalImage;

    // ���� ���� ������ �̹���
    Image m_CoolDownImage;

    protected bool m_CanSpawn = true;

    float m_CurrentIntervalTime;
    float m_InverseInterval; // ������� ���� �������ϸ� �����ϱ� ���� ����

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

        // ���� ���ð��� ������ ������ ������ �����ΰ�� Spawn �Ѵ�.
        if (CurrentIntervalTime > spawnInterval && m_CanSpawn)
        {
            CurrentIntervalTime = 0.0f;
            Spawn();
        }
    }

    //protected IEnumerator SpawnCount()
    //{
    //    while (CurrentIntervalTime < spawnInterval)
    //    {
    //        CurrentIntervalTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    Spawn();
    //}


    protected virtual void Spawn() { }
}
