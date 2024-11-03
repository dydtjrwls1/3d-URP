using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour, IPickUp, IInitialize
{
    public GameObject[] m_WeaponPrefabs;

    // weapon 이 위치할 트랜스폼
    public Transform m_WeaponPoint;

    // 투척무기 프리펩
    public GameObject grenadePrefab;

    [SerializeField]
    int maxGrenadeCount = 5;

    // 투척무기가 위치할 트랜스폼
    Transform m_GrenadePoint;

    // 투척무기 오브젝트
    GameObject m_GrenadeGameObject;

    // 현재 무기의 오브젝트
    GameObject m_CurrentWeaponPrefab;

    // 현재 투척무기 개수
    int m_CurrentGrenadeCount = 3;

    // 장전 여부 True 이면 장전 중이다 (=> 무기 변경, 총 발사 불가)
    bool m_IsReload = false;

    // 투척무기 준비 여부
    bool m_IsGrenadeReady = false;

    const int Pistol = (int)ItemCode.Pistol;
    const int Rifle = (int)ItemCode.Rifle;
    const int Shotgun = (int)ItemCode.Shotgun;

    // 현재 무기가 변경되었음을 알리는 델리게이트
    public event Action<Weapon> onWeaponChange = null;

    // 투척무기 개수 변경을 알리는 델리게이트
    public event Action<int> onGrenadeCountChange = null;

    // 투척무기 발사 준비 상태를 알리는 델리게이트
    public event Action<bool> onGrenadeReady = null;

    int CurrentGrenadeCount
    {
        get => m_CurrentGrenadeCount;
        set
        {
            if(value < 0)
            {
                m_CurrentGrenadeCount = Mathf.Clamp(value, 0, maxGrenadeCount);
                onGrenadeCountChange?.Invoke(m_CurrentGrenadeCount);
            }
        }
    }

    private void Awake()
    {
        // 무기 오브젝트 생성 후 비활성화하기
        foreach(var prefab in m_WeaponPrefabs)
        {
            GameObject obj = Instantiate(prefab, m_WeaponPoint);

            Weapon w = obj.GetComponent<Weapon>();

            // 장전 중에는 총을 바꾸지 못하게 하기 위한 조치
            w.onReload += (isReload) => { m_IsReload = isReload; }; // 장전 중일 때 isreload 가 false 가 되면서 총을 바꿀 수 없어진다.

            w.PrefabObject = obj; 
            obj.SetActive(false);
        }

        // 투척무기가 위치할 트랜스폼
        Transform child = transform.GetChild(1);
        m_GrenadePoint = child.GetChild(4);
                
        //m_Player = GetComponent<PlayerMovementContoller>();
    }

    private void Start()
    {
        // 투척무기 오브젝트 생성
        m_GrenadeGameObject = Instantiate(grenadePrefab, m_GrenadePoint);
        m_GrenadeGameObject.SetActive(false);

        PlayerInputController m_PlayerInputController = GetComponent<PlayerInputController>();

        m_PlayerInputController.onKeyOne += OnKeyOne;
        m_PlayerInputController.onKeyTwo += OnKeyTwo;
        m_PlayerInputController.onKeyThree += OnKeyThree;

        
    }

    private Weapon GetWeapon(int index)
    {
        if(index > m_WeaponPrefabs.Length - 1)
        {
            return null;
        }

        GameObject weapon = m_WeaponPoint.GetChild(index).gameObject;
        return weapon.GetComponent<Weapon>();
    }

    private bool HasWeapon(int index, out Weapon weapon)
    {
        bool result = false;

        weapon = GetWeapon(index);

        // 무기가 존재하고 무기가 활성화 되어 있을경우 true 반환
        result = weapon != null && weapon.Activate;

        return result;
    }

    public void SetWeapon(int index)
    {
        if (HasWeapon(index, out Weapon weapon))
        {
            // weapon 을 정상적으로 불러왔다면 무기를 장착한다.
            if (weapon != null && !m_IsReload && weapon.PrefabObject != m_CurrentWeaponPrefab) // 재장전 중이 아니고 지금 들고있는 무기와 같지 않을 때 
            {
                // 현재 착용중인 무기 해제
                if (m_CurrentWeaponPrefab != null)
                {
                    m_CurrentWeaponPrefab.SetActive(false);
                }

                weapon.PrefabObject.SetActive(true);
                m_CurrentWeaponPrefab = weapon.PrefabObject;

                onWeaponChange?.Invoke(weapon);
                //m_Player.SetWeaponSetting(weapon);
            }
        }
    }

    public void AddWeapon(int index)
    {
        if (!HasWeapon(index, out Weapon weapon))
        {
            weapon.Activate = true;
        }
    }

    private void OnKeyOne()
    {
        SetWeapon(Pistol);
    }

    private void OnKeyTwo()
    {
        SetWeapon(Rifle);
    }

    private void OnKeyThree()
    {
        SetWeapon(Shotgun);
    }

    private void OnGrenadeReady()
    {
        if(CurrentGrenadeCount > 0)
        {
            // 투척무기 오브젝트 활성화
            m_GrenadeGameObject.SetActive(true);

            m_IsGrenadeReady = true;

            // 투척무기 준비 완료 알림
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }

    // 투척준비상태에서 마우스 왼쪽클릭으로 발사했을 시 실행되는 함수
    private void OnGrenadeFire(bool _)
    {
        if (m_IsGrenadeReady)
        {
            m_GrenadeGameObject?.SetActive(false);

            // 투척무기 발사
            Factory.Instance.GetProjectile(Camera.main.transform.position + Camera.main.transform.forward * 0.5f);

            CurrentGrenadeCount--;
            m_IsGrenadeReady = false;
            onGrenadeReady?.Invoke(m_IsGrenadeReady);
        }
    }


    public void PickUp(ItemCode code, int capacity)
    {
        int index = (int)code;

        if (HasWeapon(index, out Weapon weapon))
        {
            if(weapon != null)
            {
                // 총알 추가
                weapon.TotalAmmo += capacity;
            }
        }
    }

    public void Initialize()
    {
        // 총 추가하기 (굳이 필요하진 않음)
        AddWeapon(Pistol);
        AddWeapon(Rifle);

        // 현재 무기 Pistol로 설정
        SetWeapon(Pistol);
    }
}

