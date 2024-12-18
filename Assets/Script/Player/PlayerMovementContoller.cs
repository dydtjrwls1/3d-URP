using Cinemachine;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovementContoller : MonoBehaviour
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
    public float firePointOffset = 0.1f;

    [Header("Recoil Setting")]
    // 사격중이 아닐 때 원래 위치로 돌아가는 정도
    public float recoilSmooth = 2.0f;

    // 좌우로 튀는 정도
    public float randomRecoilAmount = 1.0f;

    [Header("Bob Setting")]
    public float bobRate = 0.3f;
    public float aimingBobAmount = 1.1f;
    public float defaultBobAmount = 1.5f;
    public float bobRecoverySpeed = 5.0f;

    [Header("Weapon")]
    public Weapon defaultWeapon;
    public float weaponChangeOffset = 0.5f;
    public float weaponChangeSpeed = 1.5f;

    CharacterController m_controller;

    PlayerInputController m_PlayerInputController;

    CinemachineVirtualCamera m_PlayerCamera;

    Transform m_WeaponPoint;

    Weapon m_CurrentWeapon;

    Vector3 m_InputDirection;
    Vector3 m_MainLocalPosition;
    Vector3 m_RecoilWeaponPosition;
    Vector3 m_BobWeaponPosition;
    Vector3 m_WeaponChangePosition;

    bool m_IsAim = false;
    bool m_IsFire = false;
    bool m_IsFired = false;
    bool m_IsMove = false;
    bool m_IsGrenade = false;

    float m_MouseXInput;
    float m_MouseYInput;
    float m_RemainsRecoilRate = 0f;
    float m_RemainsRecoilTime = 0f;
    float m_RecoilModifier = 1.0f;
    float m_RandomRecoilX;
    float m_RandomRecoilY;
    float m_RemainsFireCoolTime = 0f;
    float m_CurrentBobTime = 0f;
    float m_CameraVerticalAngle = 0f;
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

    public Transform WeaponPoint => m_WeaponPoint;

    public Weapon CurrentWeapon => m_CurrentWeapon;

    // 무기의 남은 탄창 수가 0이 아니고 투척무기 준비상태가 아니고 마우스 오른쪽 버튼을 눌렀을 경우
    public bool CanFire => m_CurrentWeapon.CanFire && m_IsFire;

    //public event Action onAim = null;

    public event Action<Weapon> onBulletFire = null;

    // public event Action<Weapon> onWeaponChange = null;

    public event Action<float> onScoreChange = null;

    const float PI = 3.141592f;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_WeaponPoint = child.GetChild(1); // cinemachine 밑에 안보이는 자식이 있으므로 2번째 자식을 가져온다.

        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        m_controller = GetComponent<CharacterController>();

        m_PlayerInputController = GetComponent<PlayerInputController>();

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_MainLocalPosition = defaultWeaponPosition.localPosition;

        // 현재 무기 변경시 사용할 함수 연결
        PlayerWeaponHandler handler = GetComponent<PlayerWeaponHandler>();
        handler.onWeaponChange += SetWeaponSetting;
        handler.onGrenadeReady += SwapMode;

        // 플레이어 사망 시 재시작
        Health health = GetComponent<Health>();
        health.onDie += () =>
        {
            SceneController.Instance.ReloadCurrentScene();
        };

        // 마우스 움직임마다 실행되는 부분
        m_PlayerInputController.onMouseMove += (delta) =>
        {
            // HandlePlayerMoverment에서 사용할 변수 설정
            m_MouseXInput = Mathf.Clamp(delta.x, -maxRotationInputValue, maxRotationInputValue);
            m_MouseYInput = Mathf.Clamp(delta.y, -maxRotationInputValue, maxRotationInputValue);

        };

        // 플레이어 움직임이 있을때마다 실행된다
        m_PlayerInputController.onMove += (input, performed) =>
        {
            m_IsMove = performed;
            m_InputDirection = new Vector3(input.x, 0, input.y);
        };

        // 마우스 오른쪽 클릭 시 실행
        m_PlayerInputController.onRClick += () =>
        {
            m_IsAim = !m_IsAim;

            // Aim 상태일경우와 무기가 활성화 되있을 시 트리거 발동
            //if (m_IsAim && CurrentWeapon.gameObject.activeSelf)
            //{
            //    onAim?.Invoke();
            //}
        };

        // 마우스 왼쪽 클릭 시 실행
        m_PlayerInputController.onFire += (isFire) =>
        {
            // 현재 무기가 활성화 상태라면
            if (CurrentWeapon.gameObject.activeSelf)
            {
                // 수류탄 발사와 동시에 총이 발사되는것 방지용
                if (m_IsGrenade)
                {
                    m_IsGrenade = false;
                }
                else
                {
                    m_IsFire = isFire;
                }
            }
        };

        // initialize 해야하는 컴포넌트 초기화
        IInitialize[] inits = GetComponents<IInitialize>();
        if (inits.Length > 0)
        {
            foreach (IInitialize init in inits)
            {
                init.Initialize();
            }
        }
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

        if (CanFire && m_CurrentWeapon.gameObject.activeSelf)
        {
            if (m_RemainsFireCoolTime < 0f)
            {
                m_CurrentWeapon.StartFireEffectCoroutine();

                onBulletFire?.Invoke(m_CurrentWeapon);

                // 총알 발사 쿨타임 초기화
                m_RemainsFireCoolTime = CurrentWeapon.fireRate;

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
        if (!m_controller.isGrounded)
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
                m_RemainsRecoilRate = CurrentWeapon.fireRate;
                m_RemainsRecoilTime = CurrentWeapon.recoilTime;

                m_RandomRecoilX = Random.Range(-randomRecoilAmount, randomRecoilAmount);
                m_RandomRecoilY = Random.Range(-randomRecoilAmount, randomRecoilAmount);

                m_RecoilModifier = 0.75f;
            }

            // 반동 세기 계산
            float amplitude = m_IsAim ? CurrentWeapon.aimRecoilAmount : CurrentWeapon.recoilAmount;

            float xValue = Mathf.Max(0, PI * m_RecoilModifier * m_RemainsRecoilTime / CurrentWeapon.recoilTime);

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

    // 무기가 바뀔경우 실행될 함수
    public void SetWeaponSetting(Weapon weapon)
    {
        m_CurrentWeapon = weapon;
        m_WeaponChangePosition = new Vector3(0, -weaponChangeOffset, 0);
    }

    private void SwapMode(bool isGrenadeReady)
    {
        // 수류탄 발사 준비완료 상태라면 현재  weapon 을 비활성화한다.
        CurrentWeapon.gameObject.SetActive(!isGrenadeReady);

        if (isGrenadeReady)
        {
            m_IsGrenade = true;
        }

        m_IsFire = false;

        // 에임 상태에서 수류탄 준비상태 진입 시 줌아웃
        if (m_IsAim)
        {
            m_IsAim = false;
        }
    }


    void SetFOV(float fov)
    {
        m_PlayerCamera.m_Lens.FieldOfView = fov;
    }
}