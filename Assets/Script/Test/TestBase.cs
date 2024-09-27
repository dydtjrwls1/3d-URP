using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBase : MonoBehaviour
{
    TestInputAction inputAction;

    private void Awake()
    {
        inputAction = new TestInputAction();
    }

    private void OnEnable()
    {
        inputAction.Test.Enable();
        inputAction.Test.LClick.performed += LClick_performed;
        inputAction.Test.RClick.performed += RClick_performed;
        inputAction.Test.Num1.performed += Num1_performed;
        inputAction.Test.Num2.performed += Num2_performed;
        inputAction.Test.Num3.performed += Num3_performed;
        inputAction.Test.Num4.performed += Num4_performed;
        inputAction.Test.Num5.performed += Num5_performed;
    }

    private void OnDisable()
    {
        inputAction.Test.Num5.performed -= Num5_performed;
        inputAction.Test.Num4.performed -= Num4_performed;
        inputAction.Test.Num3.performed -= Num3_performed;
        inputAction.Test.Num2.performed -= Num2_performed;
        inputAction.Test.Num1.performed -= Num1_performed;
        inputAction.Test.RClick.performed -= RClick_performed;
        inputAction.Test.LClick.performed -= LClick_performed;
        inputAction.Test.Disable();
    }

    protected virtual void Num5_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void Num4_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void Num3_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void Num2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void Num1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void RClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    protected virtual void LClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }
}