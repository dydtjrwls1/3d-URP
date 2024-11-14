using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour, IInitialize
{
    public Transform healthBarPivot;

    public Image healthBar;

    PlayerMovementContoller player;

    public void Initialize()
    {
        if (player == null)
        {
            player = GameManager.Instance.Player;
        }
    }

    private void Start()
    {
        Health health = GetComponent<Health>();
        health.onHealthChange += (ratio) => { healthBar.fillAmount = ratio; };
    }

    private void Update()
    {
        healthBarPivot.LookAt(player.transform.position);

        healthBarPivot.gameObject.SetActive(healthBar.fillAmount != 1.0f);
    }

    //public void Initialize()
    //{
    //    healthBar.fillAmount = 1.0f;
    //}
}
