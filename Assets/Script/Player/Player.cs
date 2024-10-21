using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
    public float gravityForce = 9.8f;

    [Header("Weapon Position")]
    public Transform defaultWeaponPosition;
    public Transform aimingWeaponPosition;

    public float defaultFOV = 45f;
    public float FOVMultiplier = 0.8f;
    public float aimingSpeed = 2f;

    [Header("Attack Setting")]
    // public float fireRate = 0.5f;
    // public Transform m_FirePoint;
    public float firePointOffset = 0.1f;

    [Header("Recoil Setting")]
    // 반동 주기 ( 이번 반동에서 다음 반동까지의 시간 간격 , 총의 발사시간과 같게 한다.)
    // public float recoilRate = 0.5f;

    // 반동 시간 ( 이번 반동의 발동 시간 )
    //public float recoilTime = 0.1f;

    // 사격중이 아닐 때 원래 위치로 돌아가는 정도
    public float recoilSmooth = 2.0f;

    // 반동 세기
    // public float recoilAmount = 1.1f;

    // 좌우로 튀는 정도
    public float randomRecoilAmount = 1.0f;

    [Header("Bob Setting")]
    public float bobRate = 0.3f;
    public float aimingBobAmount = 1.1f;
    public float defaultBobAmount = 1.5f;
    public float bobRecoverySpeed = 5.0f;

    [Header("Shoot Effect")]
    // public ParticleSystem m_FireEffect;
    // public Light m_FireLight;
    public float lightTime = 0.5f;

    [Header("Weapon")]
    public Weapon defaultWeapon;
    public float weaponChangeOffset = 0.5f;
    public float weaponChangeSpeed = 1.5f;

    CharacterController m_controller;

    PlayerInputActions inputAction;

    GroundSensor m_GroundSensor;

    CinemachineVirtualCamera m_PlayerCamera;

    Health m_PlayerHealth;

    Transform m_WeaponPoint;
    Transform m_FirePoint;
    Transform m_PlayerWeapons;

    Coroutine m_FireLightOnCoroutune;

    List<Weapon> m_WeaponList;

    Weapon m_CurrentWeapon;
    ParticleSystem m_FireEffect;
    Light m_FireLight;

    Vector3 m_InputDirection;
    Vector3 m_MainLocalPosition;
    Vector3 m_RecoilWeaponPosition;
    Vector3 m_BobWeaponPosition;
    Vector3 m_WeaponChangePosition;

    bool m_IsAim = false;
    bool m_OnGround = true;
    bool m_IsFire = false;
    bool m_IsFired = false;
    bool m_IsMove = false;

    float m_MouseXInput;
    float m_MouseYInput;
    float m_RemainsRecoilRate = 0f;
    float m_RemainsRecoilTime = 0f;
    float m_RecoilModifier = 1.0f;
    float m_RecoilAmount;
    float m_RecoilTime;
    float m_AimRecoilAmount;
    float m_RandomRecoilX;
    float m_RandomRecoilY;
    float m_RemainsFireCoolTime = 0f;
    float m_CurrentBobTime = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_FireRate;
    float m_Score;

    public float Score
    {
        get => m_Score;
        set
        {
            if (m_Score != value)
            {
                m_Score = value;
                onScoreChange?.Invoke(m_Score);
            }
        }
    }

    public Health Health => m_PlayerHealth;

    public Weapon CurrentWeapon => m_CurrentWeapon;

    // 무기의 남은 탄창 수가 0이 아니고 마우스 오른쪽 버튼을 눌렀을 경우
    public bool CanFire => m_CurrentWeapon.CanFire && m_IsFire;

    public event Action onAim = null;

    public event Action<Weapon> onBulletFire = null;

    public event Action<Weapon> onWeaponChange = null;

    public event Action<float> onScoreChange = null;

    public event Action onKeyOne = null;
    public event Action onKeyTwo = null;
    public event Action onKeyThree = null;

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

        m_PlayerHealth = GetComponent<Health>();
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
        inputAction.Player.Num1.performed += OnKeyOne;
        inputAction.Player.Num2.performed += OnKeyTwo;
        inputAction.Player.Num3.performed += OnKeyThree;
    }

    private void OnDisable()
    {
        inputAction.Player.Num3.performed -= OnKeyThree;
        inputAction.Player.Num2.performed -= OnKeyTwo;
        inputAction.Player.Num1.performed -= OnKeyOne;
        inputAction.Player.Fire.canceled -= On_Fire;
        inputAction.Player.Fire.performed -= On_Fire;
        inputAction.Player.RClick.performed -= On_RClick;
        inputAction.Player.Jump.performed -= On_Jump;
        inputAction.Player.MousePoint.canceled -= On_MouseMove;
        inputAction.Player.MousePoint.performed -= On_MouseMove;
        inputAction.Player.Move.canceled -= OnMove;
        inputAction.Player.Move.performed -= OnMove;
        inputAction.Player.Disable();
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

        // initialize 해야하는 컴포넌트 초기화
        IInitialize[] inits = GetComponents<IInitialize>();
        if (inits.Length > 0) 
        {
            foreach (IInitialize init in inits)
            {
                init.Initialize();
            }
        }


        //AddWeapon(defaultWeapon);
        //SetWeapon(defaultWeapon);
    }

    private void Update()
    {
        HandlePlayerMovement();
        RayCastTarget();
        Fire();
    }

    private void RayCastTarget()
    {
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
        UpdateWeaponChangePosition();
        

        m_WeaponPoint.localPosition = m_MainLocalPosition + m_RecoilWeaponPosition + m_BobWeaponPosition + m_WeaponChangePosition + m_CurrentWeapon.offset;
    }

    private void UpdateWeaponChangePosition()
    {
        m_WeaponChangePosition = Vector3.Lerp(m_WeaponChangePosition, Vector3.zero, weaponChangeSpeed * Time.deltaTime);
    }

    private void Fire()
    {
        m_RemainsFireCoolTime -= Time.deltaTime;

        if (CanFire)
        {
            if(m_RemainsFireCoolTime < 0f)
            {
                if(m_FireLightOnCoroutune != null)
                {
                    m_FireLightOnCoroutune = null;
                }

                // 총알 발사 이펙트 실행
                m_FireLightOnCoroutune = StartCoroutine(OnFireEffect());

                // 총알 발사
                //Projectile projectile = Factory.Instance.GetProjectile(m_FirePoint.position + m_FirePoint.forward * firePointOffset, m_FirePoint.eulerAngles);
                //projectile.Velocity = m_FirePoint.forward;

                onBulletFire?.Invoke(m_CurrentWeapon);

                // 총알 발사 쿨타임 초기화
                m_RemainsFireCoolTime = m_FireRate;

                m_IsFired = true;
            }
        }
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

        // 공중이라면
        if(!m_controller.isGrounded)
        {
            characterVelocity += Vector3.up * -gravityForce;
        }

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
        m_RemainsRecoilRate -= Time.deltaTime;
        m_RemainsRecoilTime -= Time.deltaTime;
        m_RecoilModifier -= Time.deltaTime;

        // 발사가 성공적으로 됐다면
        if (m_IsFired)
        {
            if (m_RemainsRecoilRate < 0f)
            {
                m_RemainsRecoilRate = m_FireRate;
                m_RemainsRecoilTime = m_RecoilTime;

                m_RandomRecoilX = Random.Range(-randomRecoilAmount, randomRecoilAmount);
                m_RandomRecoilY = Random.Range(-randomRecoilAmount, randomRecoilAmount);

                m_RecoilModifier = 0.75f;
            }

            // 반동 세기 계산
            float amplitude = m_IsAim ? m_AimRecoilAmount : m_RecoilAmount;

            float xValue = Mathf.Max(0, PI * m_RecoilModifier * m_RemainsRecoilTime / m_RecoilTime);

            float recoil = Mathf.Sin(xValue) * -amplitude;

            float recoilX = recoil * m_RandomRecoilX;
            float recoilY = recoil * m_RandomRecoilY;

            // 반동 세기만큼 z축으로 이동
            Vector3 recoilPosition = new Vector3(recoilX, recoilY, recoil);

            m_RecoilWeaponPosition = recoilPosition;
        }
        else
        {
            m_RecoilWeaponPosition = Vector3.Lerp(m_RecoilWeaponPosition, Vector3.zero, recoilSmooth * Time.deltaTime);
        }

        // 반동이 끝나거나 발사버튼 미클릭 시 발사 여부는 false 로 바꾼다. 
        if (m_RemainsRecoilTime < 0f || !m_IsFire)
        {
            m_IsFired = false;
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

    //public void AddWeapon(Weapon weapon)
    //{
    //    if (HasWeapon(weapon))
    //    {
    //        return;
    //    }

    //    m_WeaponList.Add(weapon);

    //    //SetWeapon(weapon);
    //}

    //bool HasWeapon(Weapon weapon)
    //{
    //    return m_WeaponList.Contains(weapon);
    //}

    //public void SetWeapon(Weapon weapon)
    //{
    //    GameObject currentWeapon = null;

    //    if (HasWeapon(weapon))
    //    {
    //        // 같은 무기면 바꾸지 않는다.
    //        if (m_CurrentWeapon == weapon)
    //        {
    //            return;
    //        }

    //        // 현재 장착중인 무기 파괴
    //        if (m_CurrentWeapon != null)
    //        {
    //            Destroy(m_CurrentWeapon.gameObject);
    //        }

    //        // 새로 장착할 무기 생성
    //        currentWeapon = Instantiate(weapon).gameObject;

    //        m_CurrentWeapon = currentWeapon.GetComponent<Weapon>();
    //        m_FirePoint = m_CurrentWeapon.firePoint;
    //        m_FireLight = m_CurrentWeapon.FireLight;
    //        m_FireEffect = m_CurrentWeapon.fireEffect;
    //        m_RecoilAmount = m_CurrentWeapon.recoilAmount;
    //        m_RecoilTime = m_CurrentWeapon.recoilTime;
    //        m_AimRecoilAmount = m_CurrentWeapon.aimRecoilAmount;


    //        onWeaponChange?.Invoke(m_CurrentWeapon);

    //        if (currentWeapon != null)
    //        {
    //            // 무기를 weapon point에 위치
    //            currentWeapon.transform.parent = m_WeaponPoint;
    //            currentWeapon.transform.localPosition = Vector3.zero;
    //            currentWeapon.transform.forward = m_PlayerCamera.transform.forward;

    //            m_FireRate = weapon.fireRate;

    //            m_WeaponChangePosition = new Vector3(0, -weaponChangeOffset, 0);
    //        }
    //    }
    //}

    // 무기가 바뀔경우 실행될 함수
    public void SetWeaponSetting(Weapon weapon)
    {
        m_CurrentWeapon = weapon;
        m_FireRate = weapon.fireRate;
        m_FirePoint = weapon.firePoint;
        m_FireLight = weapon.FireLight;
        m_FireEffect = weapon.fireEffect;
        m_RecoilAmount = weapon.recoilAmount;
        m_RecoilTime = weapon.recoilTime;
        m_AimRecoilAmount = weapon.aimRecoilAmount;

        onWeaponChange?.Invoke(m_CurrentWeapon);

        m_WeaponChangePosition = new Vector3(0, -weaponChangeOffset, 0);
    }

    // 키보드 입력 처리 함수
    private void OnMove(InputAction.CallbackContext context)
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

    private void OnKeyOne(InputAction.CallbackContext _)
    {
        onKeyOne?.Invoke();
    }

    private void OnKeyTwo(InputAction.CallbackContext _)
    {
        onKeyTwo?.Invoke();
    }
    private void OnKeyThree(InputAction.CallbackContext _)
    {
        onKeyThree?.Invoke();
    }

    void SetFOV(float fov)
    {
        m_PlayerCamera.m_Lens.FieldOfView = fov;
    }

    IEnumerator OnFireEffect()
    {
        float elapsedTime = lightTime;

        m_FireLight.enabled = true;
        m_FireEffect.Play();

        while (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        m_FireLight.enabled = false;

        m_FireLightOnCoroutune = null;
    }


#if UNITY_EDITOR
    public void Test_CamLock()
    {
        inputAction.Player.MousePoint.performed -= On_MouseMove;
    }
#endif
}
