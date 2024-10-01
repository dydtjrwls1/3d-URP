using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test01_PlayerTest : TestBase
{
    public Player player;

    public PlayerCamera playerCamera;

    protected override void Num1_performed(InputAction.CallbackContext obj)
    {
        player.Test_CamLock();
    }

    protected override void Num2_performed(InputAction.CallbackContext obj)
    {
        playerCamera.Aim();
    }

    // shoulder offset x = 0.22
    // vretical Arm Length = 0.37
    // camera distance = 0.05
}
