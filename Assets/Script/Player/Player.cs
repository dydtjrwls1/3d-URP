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

    PlayerInputActions inputAction;

    Rigidbody rb;
    Animator animator;

    // 플레이어 Mesh 의 트랜스폼
    Transform mesh;

    Transform cameraPoint;

    GroundSensor groundSensor;

    Vector3 moveDirection;
    Vector3 inputDirection;

    Quaternion moveRotation;
    Quaternion nextCameraRotation;

    float maxCameraInputValue = 30.0f;
    float meshRotationDelta = 0.2f;

    bool isMove = false;
    bool onGround = true;

    readonly int Move_Hash = Animator.StringToHash("Move");
    readonly int Jump_Hash = Animator.StringToHash("Jump");
    readonly int IsGround_Hash = Animator.StringToHash("IsGround");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mesh = transform.GetChild(0);
        cameraPoint = transform.GetChild(1);
        groundSensor = GetComponentInChildren<GroundSensor>();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Move.canceled += OnMove;
        inputAction.Player.MousePoint.performed += On_MouseMove;
        inputAction.Player.Jump.performed += On_Jump;
    }

    private void On_Jump(InputAction.CallbackContext obj)
    {
        if (onGround)
        {
            animator.SetTrigger(Jump_Hash);
            rb.AddForce(Vector3.up * jumpForce);
        }
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
        if (isMove)
        {
            MeshRotation();
        }
    }

    private void LateUpdate()
    {
        cameraPoint.localRotation = Quaternion.Slerp(cameraPoint.localRotation, nextCameraRotation, Time.deltaTime * cameraSpeed);
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            Vector3 cameraForward = new Vector3(cameraPoint.forward.x, 0, cameraPoint.forward.z).normalized;
            moveDirection = Quaternion.LookRotation(inputDirection) * cameraForward;
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
            isMove = true;
            animator.SetBool(Move_Hash, true);
        }
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float deltaX = Mathf.Clamp(delta.x, -maxCameraInputValue, maxCameraInputValue);
        float deltaY = Mathf.Clamp(delta.y, -maxCameraInputValue, maxCameraInputValue);

        float nextXRotation;
        if (cameraPoint.localEulerAngles.x < 360.0f && cameraPoint.localEulerAngles.x > 270.0f)
        {
            nextXRotation = Mathf.Max(cameraPoint.localEulerAngles.x - deltaY, 270.1f);
        } 
        else if(cameraPoint.localEulerAngles.x > 0.0f && cameraPoint.localEulerAngles.x < 90.0f) 
        {
            nextXRotation = Mathf.Min(cameraPoint.localEulerAngles.x - deltaY, 89.9f);
        }
        else
        {
            nextXRotation = cameraPoint.localEulerAngles.x;
        }

        float nextYRotation = cameraPoint.localEulerAngles.y + deltaX;

        nextCameraRotation = Quaternion.Euler(nextXRotation, nextYRotation, 0);

        //
    }

    void MeshRotation()
    {
        Quaternion cameraForwardRotation = Quaternion.Euler(0, cameraPoint.localRotation.eulerAngles.y, 0);
        moveRotation = cameraForwardRotation * Quaternion.LookRotation(inputDirection);
        mesh.rotation = Quaternion.Lerp(mesh.rotation, moveRotation, meshRotationDelta);
    }
}
