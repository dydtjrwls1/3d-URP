using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test03_Enemy : TestBase
{
    public Transform target;

    protected override void Num1_performed(InputAction.CallbackContext obj)
    {
        Factory.Instance.GetZombie(target.position);
    }
}
