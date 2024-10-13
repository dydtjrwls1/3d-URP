using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test01_PlayerTest : TestBase
{
    public Transform firePoint;
    public Weapon rifle;
    public Weapon handGun;

    protected override void Num1_performed(InputAction.CallbackContext obj)
    {
        Time.timeScale = 0.1f;
    }

    protected override void Num2_performed(InputAction.CallbackContext obj)
    {
        Time.timeScale = 1.0f;
    }

    protected override void Num3_performed(InputAction.CallbackContext obj)
    {
        FadeManager.Instance.FadeAndLoadScene(0);
    }

    protected override void Num4_performed(InputAction.CallbackContext obj)
    {
        //GameManager.Instance.Player.AddWeapon(handGun);
        //GameManager.Instance.Player.SetWeapon(handGun);
    }

    protected override void Num5_performed(InputAction.CallbackContext obj)
    {
        //GameManager.Instance.Player.AddWeapon(rifle);
        //GameManager.Instance.Player.SetWeapon(rifle);
    }

    // shoulder offset x = 0.22
    // vretical Arm Length = 0.37
    // camera distance = 0.05
}
