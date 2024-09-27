using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test01_PlayerTest : TestBase
{
    public Player player;

    protected override void Num1_performed(InputAction.CallbackContext obj)
    {
        player.Test_CamLock();
    }

    // shoulder offset x = 0.22
    // vretical Arm Length = 0.37
    // camera distance = 0.05
}
