using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float cameraSpeed = 30.0f;
    public float jumpForce = 10.0f;

    public Transform head;

    PlayerInputActions inputAction;

    Rigidbody rb;
    Animator animator;

    // 플레이어 Mesh 의 트랜스폼
    Transform mesh;

    Transform cameraPoint;

    CinemachineVirtualCamera vcam;

    GroundSensor groundSensor;

    // 현재 카메라의 시선을 알기위한 컴포넌트
    CinemachinePOV vcamPOV;

    Vector3 moveDirection;
    Vector3 inputDirection;

    Quaternion moveRotation;
    Quaternion nextCameraRotation;

    float maxCameraInputValue = 5f;
    float meshRotationDelta = 0.2f;

    bool isMove = false;
    bool isAim = false;
    bool onGround = true;

    readonly int Move_Hash = Animator.StringToHash("Move");
    readonly int Jump_Hash = Animator.StringToHash("Jump");
    readonly int IsGround_Hash = Animator.StringToHash("IsGround");
    readonly int Aim_Hash = Animator.StringToHash("Aim");
    readonly int IsAim_Hash = Animator.StringToHash("IsAim");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();

        mesh = transform.GetChild(0);
        cameraPoint = transform.GetChild(1);
        groundSensor = GetComponentInChildren<GroundSensor>();

        vcamPOV = vcam.GetCinemachineComponent<CinemachinePOV>();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Move.canceled += OnMove;
        inputAction.Player.MousePoint.performed += On_MouseMove;
        inputAction.Player.Jump.performed += On_Jump;
        inputAction.Player.RClick.performed += On_RClick;
    }

    

    private void Start()
    {
        cameraPoint.Rotate(Vector3.right * 0.01f);
        groundSensor.onGround += (isGround) =>
        {
            onGround = isGround;
            animator.SetBool(IsGround_Hash, isGround);
        };
    }

    private void Update()
    {
        if (isAim || isMove)
        {
            MeshRotation();
        }
    }

    private void LateUpdate()
    {
        Debug.Log(cameraPoint.localRotation);
    }

    private void FixedUpdate()
    {
        //if (isJump)
        //{
        //    rb.angularVelocity = Vector3.zero;
        //}
        //else
        //{
        //    if (isMove)
        //    {
        //        Vector3 cameraForward = new Vector3(cameraPoint.forward.x, 0, cameraPoint.forward.z).normalized;
        //        moveDirection = Quaternion.LookRotation(inputDirection) * cameraForward;
        //        rb.MovePosition(rb.position + Time.fixedDeltaTime * moveSpeed * moveDirection);
        //    }
        //}
        if (isMove)
        {
            Quaternion cameraForward = Quaternion.Euler(0, vcamPOV.m_HorizontalAxis.Value, 0);
            Quaternion moveRotation = Quaternion.LookRotation(inputDirection) * cameraForward;
            moveDirection = moveRotation * transform.forward;
            rb.MovePosition(rb.position + Time.fixedDeltaTime * moveSpeed * moveDirection);
        }
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        inputDirection = new(input.x, 0, input.y);

        if (context.canceled)
        {
            animator.SetBool(Move_Hash, false);
            isMove = false;
        }
        else
        {
            animator.SetBool(Move_Hash, true);
            isMove = true;
        }
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float deltaX = Mathf.Clamp(delta.x, -maxCameraInputValue, maxCameraInputValue);
        float deltaY = Mathf.Clamp(delta.y, -maxCameraInputValue, maxCameraInputValue);

        //float nextXRotation;
        //if(cameraPoint.localEulerAngles.x - 90.0f > 0.0f &&  cameraPoint.localEulerAngles.x - 90.0f < 180.0f)
        //{
        //    nextXRotation = cameraPoint.localEulerAngles.x;
        //}
        //else
        //{
        //    nextXRotation = cameraPoint.localEulerAngles.x - deltaY;
        //}

        //if (cameraPoint.localEulerAngles.x < 360.0f && cameraPoint.localEulerAngles.x > 270.0f)
        //{
        //    nextXRotation = Mathf.Max(cameraPoint.localEulerAngles.x - deltaY, 270.1f);
        //} 
        //else if(cameraPoint.localEulerAngles.x > 0.0f && cameraPoint.localEulerAngles.x < 90.0f) 
        //{
        //    nextXRotation = Mathf.Min(cameraPoint.localEulerAngles.x - deltaY, 89.9f);
        //}
        //else
        //{
        //    nextXRotation = cameraPoint.localEulerAngles.x;
        //}
        float nextXRotation = cameraPoint.localEulerAngles.x - deltaY;
        float nextYRotation = cameraPoint.localEulerAngles.y + deltaX;

        nextCameraRotation = Quaternion.Euler(nextXRotation, nextYRotation, 0);

        //
    }

    private void On_RClick(InputAction.CallbackContext obj)
    {
        isAim = !isAim;
        animator.SetBool(IsAim_Hash, isAim);

        // Aim 상태일경우 트리거 발동
        if (isAim)
        {
            AimCameraSetting();
            animator.SetTrigger(Aim_Hash);
        }
    }
    private void On_Jump(InputAction.CallbackContext obj)
    {
        if (onGround)
        {
            animator.SetTrigger(Jump_Hash);
            rb.velocity = new Vector3(jumpForce, jumpForce);
        }
    }

    void MeshRotation()
    {
        Quaternion cameraForwardRotation = Quaternion.Euler(0, vcamPOV.m_HorizontalAxis.Value, 0);
        //moveRotation = cameraForwardRotation * Quaternion.LookRotation(inputDirection);
        mesh.localRotation = cameraForwardRotation;
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
