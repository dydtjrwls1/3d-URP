using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    PlayerInputActions inputAction;

    public event Action<Vector2, bool> onMove = null;
    public event Action<Vector2> onMouseMove = null;
    public event Action onRClick = null;
    public event Action<bool> onFire = null;
    public event Action onKeyOne = null;
    public event Action onKeyTwo = null;
    public event Action onKeyThree = null;
    public event Action onGrenade = null;


    private void Awake()
    {
        inputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Move.canceled += OnMove;
        inputAction.Player.MousePoint.performed += On_MouseMove;
        inputAction.Player.MousePoint.canceled += On_MouseMove;
        inputAction.Player.RClick.performed += On_RClick;
        inputAction.Player.Fire.performed += On_Fire;
        inputAction.Player.Fire.canceled += On_Fire;
        inputAction.Player.Num1.performed += OnKeyOne;
        inputAction.Player.Num2.performed += OnKeyTwo;
        inputAction.Player.Num3.performed += OnKeyThree;
        inputAction.Player.Grenade.performed += OnGrenade;
    }
    private void OnDisable()
    {
        inputAction.Player.Grenade.performed -= OnGrenade;
        inputAction.Player.Num3.performed -= OnKeyThree;
        inputAction.Player.Num2.performed -= OnKeyTwo;
        inputAction.Player.Num1.performed -= OnKeyOne;
        inputAction.Player.Fire.canceled -= On_Fire;
        inputAction.Player.Fire.performed -= On_Fire;
        inputAction.Player.RClick.performed -= On_RClick;
        inputAction.Player.MousePoint.canceled -= On_MouseMove;
        inputAction.Player.MousePoint.performed -= On_MouseMove;
        inputAction.Player.Move.canceled -= OnMove;
        inputAction.Player.Move.performed -= OnMove;
        inputAction.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        onMove.Invoke(input, !context.canceled);
    }

    // 마우스 입력 처리 함수
    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        onMouseMove?.Invoke(delta);
    }

    private void On_RClick(InputAction.CallbackContext _)
    {
        onRClick?.Invoke();
    }
    private void On_Fire(InputAction.CallbackContext context)
    {
        onFire?.Invoke(!context.canceled);
    }

    //private void OnKey(InputAction.CallbackContext obj)
    //{
    //    obj.ReadValue
    //}

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


    private void OnGrenade(InputAction.CallbackContext _)
    {
        onGrenade?.Invoke();
    }

    public void ActivateInputSystem()
    {
        inputAction.Player.Enable();
    }

    public void DeActivateInputSystem()
    {
        inputAction.Player.Disable();
    }
}
