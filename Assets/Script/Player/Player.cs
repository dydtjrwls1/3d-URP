using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 30.0f;

    public float maxRotationInputValue = 5.0f;

    [Header("Player Physics Settings")]
    public float jumpForce = 10.0f;
    public float maxPlayerSpeed = 10.0f;
    public float velocityChangeSpeed = 10.0f;

    [Header("Weapon Position")]
    public Transform defaultWeaponPosition;
    public Transform aimingWeaponPosition;

    public float defaultFOV = 45f;
    public float FOVMultiplier = 0.8f;
    public float aimingSpeed = 2f;

    [Header("Attack Setting")]
    // public float fireRate = 0.5f;
    public Transform firePoint;
    public float firePointOffset = 0.1f;

    [Header("Recoil Setting")]
    // 반동 주기 ( 이번 반동에서 다음 반동까지의 시간 간격 )
    public float recoilRate = 0.5f;

    // 반동 시간 ( 이번 반동의 발동 시간 )
    public float recoilTime = 0.1f;

    // 사격중이 아닐 때 원래 위치로 돌아가는 정도
    public float recoilSmooth = 2.0f;

    // 반동 세기
    public float recoilAmount = 1.1f;

    [Header("Bob Setting")]
    public float bobRate = 0.3f;
    public float aimingBobAmount = 1.1f;
    public float defaultBobAmount = 1.5f;
    public float bobRecoverySpeed = 5.0f;

    [Header("Shoot Effect")]
    public ParticleSystem shootEffect;
    public Light fireLight;
    public float lightTime = 0.5f;

    [Header("Weapon")]
    public Weapon defaultWeapon;

    CharacterController m_controller;

    PlayerInputActions inputAction;

    GroundSensor m_GroundSensor;

    CinemachineVirtualCamera m_PlayerCamera;

    Transform m_WeaponPoint;

    Coroutine m_FireLightOnCoroutune;

    List<Weapon> m_WeaponList;

    Weapon m_CurrentWeapon;

    Vector3 m_WeaponOffset;
    Vector3 m_InputDirection;
    Vector3 m_MainLocalPosition;
    Vector3 m_RecoilWeaponPosition;
    Vector3 m_BobWeaponPosition;

    bool m_IsAim = false;
    bool m_OnGround = true;
    bool m_IsFire = false;
    bool m_IsMove = false;

    float m_MouseXInput;
    float m_MouseYInput;
    float m_CurrentRecoilRate = 0f;
    float m_CurrentRecoilTime = 0f;
    float m_CurrentFireCoolTime = 0f;
    float m_CurrentBobTime = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_FireRate;

    public Action onAim = null;

    public Action onBulletFire = null;

    const float PI = 3.141592f;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_WeaponPoint = child.GetChild(1); // cinemachine 밑에 안보이는 자식이 있으므로 2번째 자식을 가져온다.

        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        m_controller = GetComponent<CharacterController>();

        inputAction = new PlayerInputActions();

        m_GroundSensor = GetComponentInChildren<GroundSensor>();

        m_WeaponList = new List<Weapon>();
    }

   

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Move.canceled += OnMove;
        inputAction.Player.MousePoint.performed += On_MouseMove;
        inputAction.Player.MousePoint.canceled += On_MouseMove;
        inputAction.Player.Jump.performed += On_Jump;
        inputAction.Player.RClick.performed += On_RClick;
        inputAction.Player.Fire.performed += On_Fire;
        inputAction.Player.Fire.canceled += On_Fire;
    }

    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //cameraPoint.Rotate(Vector3.right * 0.01f);
        m_GroundSensor.onGround += (isGround) =>
        {
            m_OnGround = isGround;
        };

        m_MainLocalPosition = defaultWeaponPosition.localPosition;

        AddWeapon(defaultWeapon);
        SetWeapon(defaultWeapon);
    }

    private void Update()
    {
        HandlePlayerMovement();
        Fire();
    }

    private void LateUpdate()
    {
        HandleGunMovement();
    }

    private void HandleGunMovement()
    {
        UpdateAimingPosition();
        UpdateRecoilPosition();
        UpdateBobPosition();
        

        m_WeaponPoint.localPosition = m_MainLocalPosition + m_RecoilWeaponPosition + m_BobWeaponPosition;
    }

    private void Fire()
    {
        if (m_IsFire)
        {
            if(m_CurrentFireCoolTime < 0)
            {
                if(m_FireLightOnCoroutune != null)
                {
                    m_FireLightOnCoroutune = null;
                }

                // 총알 발사 이펙트 실행
                m_FireLightOnCoroutune = StartCoroutine(OnFireEffect());

                // 총알 발사
                Projectile projectile = Factory.Instance.GetProjectile(firePoint.position + firePoint.forward * firePointOffset, firePoint.eulerAngles);
                projectile.Velocity = firePoint.forward;

                onBulletFire?.Invoke();

                // 총알 발사 쿨타임 초기화
                m_CurrentFireCoolTime = m_FireRate;
            }
        }

        m_CurrentFireCoolTime -= Time.deltaTime;
    }

    // 플레이어 프레임별 움직임
    private void HandlePlayerMovement()
    {
        // 수평 회전
        transform.Rotate(Vector3.up * m_MouseXInput * rotationSpeed, Space.Self);

        // 카메라 수직 회전
        m_CameraVerticalAngle -= m_MouseYInput * rotationSpeed;

        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

        m_PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);

        // 플레이어 이동
        Vector3 characterVelocity = transform.TransformVector(m_InputDirection) * moveSpeed;

        m_controller.Move(characterVelocity * Time.deltaTime);
    }

    private void UpdateBobPosition()
    {
        m_CurrentBobTime -= Time.deltaTime;

        if (m_IsMove)
        {
            
            if (m_CurrentBobTime < 0)
            {
                m_CurrentBobTime = bobRate;
            }

            float bob = Math.Max(0, 2 * PI * m_CurrentBobTime / bobRate);
            float bobAmount = m_IsAim ? aimingBobAmount : defaultBobAmount;
            float verticalBobValue = (Mathf.Sin(bob * 2f) + 1f) * 0.5f * bobAmount;
            float horizontalBobValue = Mathf.Sin(bob) * 1.5f * bobAmount;

            Vector3 bobPosition = new Vector3(horizontalBobValue, verticalBobValue, 0) * 0.005f;

            m_BobWeaponPosition = bobPosition;
        }
        else
        {
            m_BobWeaponPosition = Vector3.Lerp(m_BobWeaponPosition, Vector3.zero, bobRecoverySpeed * Time.deltaTime);
        }

    }

    // 총알을 발사하고 총의 반동에 의한 움직임을 동작하는 함수
    private void UpdateRecoilPosition()
    {
        m_CurrentRecoilRate -= Time.deltaTime;
        m_CurrentRecoilTime -= Time.deltaTime;

        if (m_IsFire)
        {
            if (m_CurrentRecoilRate < 0f)
            {
                m_CurrentRecoilRate = recoilRate;
                m_CurrentRecoilTime = recoilTime;
            }

            // 반동 세기 계산
            float amplitude = -recoilAmount;

            float xValue = Mathf.Max(0, PI * m_CurrentRecoilTime / recoilTime);

            float recoil = Mathf.Sin(xValue) * amplitude;
            

            // 반동 세기만큼 z축으로 이동
            Vector3 recoilPosition = new Vector3(0, 0, recoil);

            m_RecoilWeaponPosition = recoilPosition;
        }
        else
        {
            m_RecoilWeaponPosition = Vector3.Lerp(m_RecoilWeaponPosition, Vector3.zero, recoilSmooth * Time.deltaTime);
        }
        
    }

    // 조준 시 위치를 변경하는 함수
    private void UpdateAimingPosition()
    {
        if (m_IsAim)
        {
            m_MainLocalPosition = Vector3.Lerp(m_MainLocalPosition, aimingWeaponPosition.localPosition, aimingSpeed * Time.deltaTime);
            SetFOV(Mathf.Lerp(m_PlayerCamera.m_Lens.FieldOfView, defaultFOV * FOVMultiplier, aimingSpeed * Time.deltaTime));
        }
        else
        {
            m_MainLocalPosition = Vector3.Lerp(m_MainLocalPosition, defaultWeaponPosition.localPosition, aimingSpeed * Time.deltaTime);
            SetFOV(Mathf.Lerp(m_PlayerCamera.m_Lens.FieldOfView, defaultFOV, aimingSpeed * Time.deltaTime));
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if (HasWeapon(weapon))
        {
            return;
        }

        m_WeaponList.Add(weapon);

        //SetWeapon(weapon);
    }

    bool HasWeapon(Weapon weapon)
    {
        return m_WeaponList.Contains(weapon);
    }

    public void SetWeapon(Weapon weapon)
    {
        GameObject currentWeapon = null;

        if (HasWeapon(weapon))
        {
            // 현재 장착중인 무기 파괴
            if (m_CurrentWeapon != null)
            {
                Destroy(m_CurrentWeapon.gameObject);
            }

            // 새로 장착할 무기 생성
            currentWeapon = Instantiate(weapon).gameObject;
            m_CurrentWeapon = currentWeapon.GetComponent<Weapon>();
        }

        if(currentWeapon != null)
        {
            // 무기를 weapon point에 위치
            currentWeapon.transform.parent = m_WeaponPoint;
            currentWeapon.transform.localPosition = Vector3.zero + weapon.offset;
            currentWeapon.transform.forward = m_PlayerCamera.transform.forward;
            
            m_FireRate = weapon.fireRate;
        }
    }

    // 키보드 입력 처리 함수
    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        m_IsMove = !context.canceled;

        Vector2 input = context.ReadValue<Vector2>();
        m_InputDirection = new Vector3(input.x, 0, input.y);
    }

    // 마우스 입력 처리 함수
    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        // 마우스 입력 최대, 최소값 설정
        m_MouseXInput = Mathf.Clamp(delta.x, -maxRotationInputValue, maxRotationInputValue);
        m_MouseYInput = Mathf.Clamp(delta.y, -maxRotationInputValue, maxRotationInputValue);
    }

    private void On_RClick(InputAction.CallbackContext obj)
    {
        m_IsAim = !m_IsAim;

        // Aim 상태일경우 트리거 발동
        if (m_IsAim)
        {
            onAim?.Invoke();
        }
    }
    private void On_Jump(InputAction.CallbackContext obj)
    {
        if (m_OnGround)
        {
            //m_Rigidbody.velocity = new Vector3(jumpForce, jumpForce);
        }
    }

    private void On_Fire(InputAction.CallbackContext context)
    {
        m_IsFire = !context.canceled;
    }

    
   
    void SetFOV(float fov)
    {
        m_PlayerCamera.m_Lens.FieldOfView = fov;
    }

    IEnumerator OnFireEffect()
    {
        float elapsedTime = lightTime;

        fireLight.enabled = true;
        shootEffect.Play();

        while (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        fireLight.enabled = false;

        m_FireLightOnCoroutune = null;
    }


#if UNITY_EDITOR
    public void Test_CamLock()
    {
        inputAction.Player.MousePoint.performed -= On_MouseMove;
    }
#endif
}
