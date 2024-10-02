using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float cameraSpeed = 30.0f;
    public float jumpForce = 10.0f;

    public float maxPlayerSpeed = 10.0f;

    public float velocityChangeSpeed = 10.0f;

    public Transform head;

    public CinemachineVirtualCamera vcam;

    PlayerInputActions inputAction;

    Rigidbody rb;

    GroundSensor groundSensor;

    // 현재 카메라의 시선을 알기위한 컴포넌트
    CinemachinePOV vcamPOV;

    Vector2 inputDirection;

    bool isMove = false;
    bool isAim = false;
    bool onGround = true;

    public Action onAim = null;

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        //mesh = transform.GetChild(0);
        //cameraPoint = transform.GetChild(1);
        groundSensor = GetComponentInChildren<GroundSensor>();

        vcamPOV = vcam.GetCinemachineComponent<CinemachinePOV>();
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
        groundSensor.onGround += (isGround) =>
        {
            onGround = isGround;
        };
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        rb.rotation = Quaternion.Euler(Vector3.up * vcamPOV.m_HorizontalAxis.Value);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxPlayerSpeed);

        Vector3 inputForce = ((transform.forward * inputDirection.y) + (transform.right * inputDirection.x)).normalized * moveSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, inputForce, Time.fixedDeltaTime * velocityChangeSpeed);
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float xMove = Mathf.Clamp(delta.x, -30.0f, 30.0f);
        float yMove = Mathf.Clamp(delta.y, -30.0f, 30.0f);

        vcamPOV.m_HorizontalAxis.m_InputAxisValue = xMove;
        vcamPOV.m_VerticalAxis.m_InputAxisValue = yMove;
    }

    private void On_RClick(InputAction.CallbackContext obj)
    {
        isAim = !isAim;

        // Aim 상태일경우 트리거 발동
        if (isAim)
        {
            onAim?.Invoke();
        }
    }
    private void On_Jump(InputAction.CallbackContext obj)
    {
        if (onGround)
        {
            rb.velocity = new Vector3(jumpForce, jumpForce);
        }
    }

    void AimCameraSetting()
    {
        Cinemachine3rdPersonFollow component = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (component != null) 
        {
            component.ShoulderOffset = new Vector3(0.22f, 0.36f, 0.0f);
            component.VerticalArmLength = -0.33f;
            component.CameraDistance = 0.05f;
        }
        else
        {
            Debug.LogWarning("Cinemachine3rdPersonFollow 컴포넌트를 불러오지 못했습니다.");
        }
    }

#if UNITY_EDITOR
    public void Test_CamLock()
    {
        inputAction.Player.MousePoint.performed -= On_MouseMove;
    }
#endif
}
