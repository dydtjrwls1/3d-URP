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
    public float fireRate = 0.5f;
    public float recoilTime = 0.1f;
    public float recoilSmooth = 2.0f;
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

    CharacterController m_controller;

    PlayerInputActions inputAction;

    GroundSensor m_GroundSensor;

    CinemachineVirtualCamera m_PlayerCamera;

    Transform m_WeaponPoint;

    Coroutine m_FireLightOnCoroutune;

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
    float m_CurrentRecoilTime = 0f;
    float m_CurrentFireCoolTime = 0f;
    float m_CurrentBobTime = 0f;
    float m_CameraVerticalAngle = 0f;

    public Action onAim = null;

    const float PI = 3.141592f;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_WeaponPoint = child.GetChild(1); // cinemachine �ؿ� �Ⱥ��̴� �ڽ��� �����Ƿ� 2��° �ڽ��� �����´�.

        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        m_controller = GetComponent<CharacterController>();

        inputAction = new PlayerInputActions();

        m_GroundSensor = GetComponentInChildren<GroundSensor>();
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
    }

    private void Update()
    {
        HandlePlayerMovement();
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
        Fire();

        m_WeaponPoint.localPosition = m_MainLocalPosition + m_RecoilWeaponPosition + m_BobWeaponPosition;
    }

    private void Fire()
    {
        if (m_IsFire)
        {
            if(m_CurrentFireCoolTime < 0)
            {
                // �Ѿ� �߻�

                if(m_FireLightOnCoroutune != null)
                {
                    m_FireLightOnCoroutune = null;
                }

                m_FireLightOnCoroutune = StartCoroutine(OnFireEffect());

                // �Ѿ� �߻� ��Ÿ�� �ʱ�ȭ
                m_CurrentFireCoolTime = fireRate;
            }
        }

        m_CurrentFireCoolTime -= Time.deltaTime;
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
        m_CurrentRecoilTime -= Time.deltaTime;

        if (m_IsFire)
        {
            if (m_CurrentFireCoolTime < 0f)
            {
                m_CurrentRecoilTime = recoilTime;
            }

            // �ݵ� ���� ���
            float amplitude = -recoilAmount;

            float xValue = Mathf.Max(0, PI * m_CurrentRecoilTime / recoilTime);

            float recoil = Mathf.Sin(xValue) * amplitude;
            

            // �ݵ� ���⸸ŭ z������ �̵�
            Vector3 recoilPosition = new Vector3(0, 0, recoil);

            m_RecoilWeaponPosition = recoilPosition;
        }
        else
        {
            m_RecoilWeaponPosition = Vector3.Lerp(m_RecoilWeaponPosition, Vector3.zero, recoilSmooth * Time.deltaTime);
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

    // Ű���� �Է� ó�� �Լ�
    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        m_IsMove = !context.canceled;

        Vector2 input = context.ReadValue<Vector2>();
        m_InputDirection = new Vector3(input.x, 0, input.y);
    }

    // ���콺 �Է� ó�� �Լ�
    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        // ���콺 �Է� �ִ�, �ּҰ� ����
        m_MouseXInput = Mathf.Clamp(delta.x, -maxRotationInputValue, maxRotationInputValue);
        m_MouseYInput = Mathf.Clamp(delta.y, -maxRotationInputValue, maxRotationInputValue);
    }

    private void On_RClick(InputAction.CallbackContext obj)
    {
        m_IsAim = !m_IsAim;

        // Aim �����ϰ�� Ʈ���� �ߵ�
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
