using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    CharacterController m_controller;

    PlayerInputActions inputAction;

    GroundSensor m_GroundSensor;

    CinemachineVirtualCamera m_PlayerCamera;

    Vector3 m_InputDirection;

    bool m_IsMove = false;
    bool m_IsAim = false;
    bool m_OnGround = true;

    float m_MouseXInput;
    float m_MouseYInput;

    float m_CameraVerticalAngle = 0f;

    public Action onAim = null;

    private void Awake()
    {
        m_PlayerCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        m_controller = GetComponent<CharacterController>();

        inputAction = new PlayerInputActions();

        //mesh = transform.GetChild(0);
        //cameraPoint = transform.GetChild(1);
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
    }

    private void Update()
    {
        HandlePlayerMovement();
    }
    private void FixedUpdate()
    {
        //m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, maxPlayerSpeed);

        //Vector3 inputForce = ((transform.forward * m_InputDirection.y) + (transform.right * m_InputDirection.x)).normalized * moveSpeed;
        //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, inputForce, Time.fixedDeltaTime * velocityChangeSpeed);
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        m_InputDirection = new Vector3(input.x, 0, input.y);
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

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

        Debug.Log(characterVelocity);

        m_controller.Move(characterVelocity * Time.deltaTime);
    }
   

#if UNITY_EDITOR
    public void Test_CamLock()
    {
        inputAction.Player.MousePoint.performed -= On_MouseMove;
    }
#endif
}
