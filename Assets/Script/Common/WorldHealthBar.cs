using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    public Transform healthBarPivot;

    public Image healthBar;

    PlayerMovementContoller player;

    private void Start()
    {
        player = GameManager.Instance.Player;

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
