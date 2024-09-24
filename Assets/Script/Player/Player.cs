using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float cameraSpeed = 30.0f;

    PlayerInputActions inputAction;

    Rigidbody rb;
    Animator animator;

    // 플레이어 Mesh 의 트랜스폼
    Transform mesh;

    Transform cameraPoint;

    Vector3 moveDirection;
    Vector3 inputDirection;

    Quaternion moveRotation;
    Quaternion nextCameraRotation;

    bool isMove = false;

    readonly int Move_Hash = Animator.StringToHash("Move");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mesh = transform.GetChild(0);
        cameraPoint = transform.GetChild(1);
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Move.canceled += OnMove;
        inputAction.Player.MousePoint.performed += On_MouseMove;
    }

    

    private void Start()
    {
        
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
            //moveRotation = cameraPoint.localRotation * Quaternion.LookRotation(inputDirection);
            //moveRotation = Quaternion.Euler(0, moveRotation.y, 0);
            //Quaternion cameraForwardRotation = Quaternion.Euler(0, cameraPoint.localRotation.eulerAngles.y, 0);
            //moveRotation = cameraForwardRotation * Quaternion.LookRotation(inputDirection);
            //moveDirection = moveRotation * cameraPoint.forward;
            isMove = true;
            animator.SetBool(Move_Hash, true);
        }
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float deltaX = Mathf.Clamp(delta.x, -30.0f, 30.0f);
        float deltaY = Mathf.Clamp(delta.y, -30.0f, 30.0f);

        float nextXRotation = cameraPoint.localEulerAngles.x - deltaY;
        float nextYRotation = cameraPoint.localEulerAngles.y + deltaX;

        nextCameraRotation = Quaternion.Euler(nextXRotation, nextYRotation, 0);

        //
    }

    void MeshRotation()
    {
        Quaternion cameraForwardRotation = Quaternion.Euler(0, cameraPoint.localRotation.eulerAngles.y, 0);
        moveRotation = cameraForwardRotation * Quaternion.LookRotation(inputDirection);
        mesh.rotation = Quaternion.Lerp(mesh.rotation, moveRotation, 0.05f);
    }
}
