using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test03_Enemy : TestBase
{
    public Transform target;

    public SpawnPoint_Trigger trigger;

    protected override void Num1_performed(InputAction.CallbackContext obj)
    {
        GameManager.Instance.Player.GetComponent<Health>().OnDamage(500.0f);
    }

    protected override void Num2_performed(InputAction.CallbackContext obj)
    {
        Factory.Instance.GetEnemy(target.position, EnemyType.ZombieWeapon);
    }

    protected override void Num3_performed(InputAction.CallbackContext obj)
    {
        Factory.Instance.GetEnemy(target.position, EnemyType.Power);
    }

    protected override void Num4_performed(InputAction.CallbackContext obj)
    {
        // LoadingManager loadingManager = FindAnyObjectByType<LoadingManager>();
        // if (loadingManager != null)
        // {
        //     loadingManager.LoadScene(1);
        // }

        SceneController.Instance.ReloadCurrentScene();
    }

    protected override void Num5_performed(InputAction.CallbackContext obj)
    {
        HealthPresenter presenter = GameManager.Instance.Player.GetComponentInChildren<HealthPresenter>();
        presenter.OnDamage(60.0f);
    }
}
