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
    // ������� �ƴ� �� ���� ��ġ�� ���ư��� ����
    public float recoilSmooth = 2.0f;

    // �¿�� Ƣ�� ����
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

    PlayerGrenadeHandler m_PlayerGrenadeHandler;

    CinemachineVirtualCamera m_PlayerCamera;

    Transform m_WeaponPoint;

    Coroutine m_FireLightOnCoroutune;

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

    public PlayerGrenadeHandler GrenadeHandler => m_PlayerGrenadeHandler;

    public Weapon CurrentWeapon => m_CurrentWeapon;

    // ������ ���� źâ ���� 0�� �ƴϰ� ��ô���� �غ���°� �ƴϰ� ���콺 ������ ��ư�� ������ ���
    public bool CanFire => m_CurrentWeapon.CanFire && m_IsFire;

    public event Action onAim = null;

    public event Action onGrenadeFire = null;

    public event Action<Weapon> onBulletFire = null;

    public event Action<Weapon> onWeaponChange = null;

    public event Action<float> onScoreChange = null;

    public event Action onKeyOne = null;
    public event Action onKeyTwo = null;
    public event Action onKeyThree = null;

    public event Action onGrenade = null;

    const float PI = 3.141592f;


    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_WeaponPoint = child.GetChild(1); // cinemachine �ؿ� �Ⱥ��̴� �ڽ��� �����Ƿ� 2��° �ڽ��� �����´�.

        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        m_controller = GetComponent<CharacterController>();

        m_PlayerInputController = GetComponent<PlayerInputController>();

        m_PlayerGrenadeHandler = GetComponent<PlayerGrenadeHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_MainLocalPosition = defaultWeaponPosition.localPosition;

        // initialize �ؾ��ϴ� ������Ʈ �ʱ�ȭ
        IInitialize[] inits = GetComponents<IInitialize>();
        if (inits.Length > 0) 
        {
            foreach (IInitialize init in inits)
            {
                init.Initialize();
            }
        }

        // ���콺 �����Ӹ��� ����Ǵ� �κ�
        m_PlayerInputController.onMouseMove += (delta) =>
        {
            m_MouseXInput = Mathf.Clamp(delta.x, -maxRotationInputValue, maxRotationInputValue);
            m_MouseYInput = Mathf.Clamp(delta.y, -maxRotationInputValue, maxRotationInputValue);
        };

        // �÷��̾� �������� ���������� ����ȴ�
        m_PlayerInputController.onMove += (input, performed) =>
        {
            m_IsMove = performed;
            m_InputDirection = new Vector3(input.x, 0, input.y);
        };

        // ���콺 ������ Ŭ�� �� ����
        m_PlayerInputController.onRClick += () =>
        {
            m_IsAim = !m_IsAim;

            // Aim �����ϰ�� Ʈ���� �ߵ�
            if (m_IsAim)
            {
                onAim?.Invoke();
            }
        };

        // ���콺 ���� Ŭ�� �� ����
        m_PlayerInputController.onFire += (isFire) =>
        {
            if (!CurrentWeapon.gameObject.activeSelf && m_PlayerGrenadeHandler.IsGrenadeReady)
            {
                onGrenadeFire?.Invoke();
                CurrentWeapon.gameObject.SetActive(true);
            }
            else
            {
                m_IsFire = isFire;
            }
        };

        m_PlayerInputController.onKeyOne += OnKeyOne;
        m_PlayerInputController.onKeyTwo += OnKeyTwo;
        m_PlayerInputController.onKeyThree += OnKeyThree;

        m_PlayerInputController.onGrenade += OnGrenade;
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

        if (CanFire)
        {
            if(m_RemainsFireCoolTime < 0f)
            {
                //if(m_FireLightOnCoroutune != null)
                //{
                //    m_FireLightOnCoroutune = null;
                //}

                //// �Ѿ� �߻� ����Ʈ ����
                //m_FireLightOnCoroutune = StartCoroutine(OnFireEffect());

                m_CurrentWeapon.StartFireEffectCoroutine();

                onBulletFire?.Invoke(m_CurrentWeapon);

                // �Ѿ� �߻� ��Ÿ�� �ʱ�ȭ
                m_RemainsFireCoolTime = CurrentWeapon.fireRate;

                m_IsFired = true;
            }
        }
    }

    // �÷��̾� �����Ӻ� ������
    private void HandlePlayerMovement()
    {
        // ���� ȸ��
        transform.Rotate(Vector3.up * m_MouseXInput * rotationSpeed, Space.Self);

        // ī�޶� ���� ȸ��
        m_CameraVerticalAngle -= m_MouseYInput * rotationSpeed;

        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

        m_PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);

        // �÷��̾� �̵�
        Vector3 characterVelocity = transform.TransformVector(m_InputDirection) * moveSpeed;

        // �����̶��
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

    // �Ѿ��� �߻��ϰ� ���� �ݵ��� ���� �������� �����ϴ� �Լ�
    private void UpdateRecoilPosition()
    {
        m_RemainsRecoilRate -= Time.deltaTime;
        m_RemainsRecoilTime -= Time.deltaTime;
        m_RecoilModifier -= Time.deltaTime;

        // �߻簡 ���������� �ƴٸ�
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

            // �ݵ� ���� ���
            float amplitude = m_IsAim ? CurrentWeapon.aimRecoilAmount : CurrentWeapon.recoilAmount;

            float xValue = Mathf.Max(0, PI * m_RecoilModifier * m_RemainsRecoilTime / CurrentWeapon.recoilTime);

            float recoil = Mathf.Sin(xValue) * -amplitude;

            float recoilX = recoil * m_RandomRecoilX;
            float recoilY = recoil * m_RandomRecoilY;

            // �ݵ� ���⸸ŭ z������ �̵�
            Vector3 recoilPosition = new Vector3(recoilX, recoilY, recoil);

            m_RecoilWeaponPosition = recoilPosition;
        }
        else
        {
            m_RecoilWeaponPosition = Vector3.Lerp(m_RecoilWeaponPosition, Vector3.zero, recoilSmooth * Time.deltaTime);
        }

        // �ݵ��� �����ų� �߻��ư ��Ŭ�� �� �߻� ���δ� false �� �ٲ۴�. 
        if (m_RemainsRecoilTime < 0f || !m_IsFire)
        {
            m_IsFired = false;
        }
    }

    // ���� �� ��ġ�� �����ϴ� �Լ�
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

    // ���Ⱑ �ٲ��� ����� �Լ�
    public void SetWeaponSetting(Weapon weapon)
    {
        m_CurrentWeapon = weapon;
        onWeaponChange?.Invoke(m_CurrentWeapon);

        m_WeaponChangePosition = new Vector3(0, -weaponChangeOffset, 0);
    }

    private void OnKeyOne()
    {
        onKeyOne?.Invoke();
    }

    private void OnKeyTwo()
    {
        onKeyTwo?.Invoke();
    }
    private void OnKeyThree()
    {
        onKeyThree?.Invoke();
    }

    private void OnGrenade()
    {
        onGrenade?.Invoke();

        // ������ ���̰ų� �̹� ��ô�غ� �����ϰ��� �ش� �ȵ�
        if (m_CurrentWeapon.CurrentAmmo != 0 && m_PlayerGrenadeHandler.IsGrenadeReady)
        {
            // ���� ���� �ൿ ��� ����
            CurrentWeapon.gameObject.SetActive(false);
        }
    }

    void SetFOV(float fov)
    {
        m_PlayerCamera.m_Lens.FieldOfView = fov;
    }

    //IEnumerator OnFireEffect()
    //{
    //    float elapsedTime = lightTime;

    //    CurrentWeapon.FireLight.enabled = true;
    //    CurrentWeapon.fireEffect.Play();

    //    while (elapsedTime > 0f)
    //    {
    //        elapsedTime -= Time.deltaTime;
    //        yield return null;
    //    }

    //    CurrentWeapon.FireLight.enabled = false;

    //    m_FireLightOnCoroutune = null;
    //}


#if UNITY_EDITOR
    //public void Test_CamLock()
    //{
    //    inputAction.Player.MousePoint.performed -= On_MouseMove;
    //}
#endif
}
