using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    TextMeshProUGUI m_MaxAmmoGUI;
    TextMeshProUGUI m_CurrentAmmoGUI;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        m_MaxAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        m_CurrentAmmoGUI = child.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Player player = GameManager.Instance.Player;

        player.onWeaponChange += UpdateMaxAmmoDisplay;

        // player 의 setWeapon 함수가 시작한 뒤에 UpdateMaxAmmoDisplay 함수가 등록된다. 실행 타이밍이 안맞기 때문에 Update를 한번 해준다.
        UpdateMaxAmmoDisplay(player.CurrentWeapon);
        UpdateCurrentAmmoDisplay(player.CurrentWeapon.maxAmmo);
    }

    private void UpdateCurrentAmmoDisplay(int currentBullets)
    {
        m_CurrentAmmoGUI.text = currentBullets.ToString();
    }

    private void UpdateMaxAmmoDisplay(Weapon weapon)
    {
        m_MaxAmmoGUI.text = weapon.maxAmmo.ToString();
        m_CurrentAmmoGUI.text = weapon.maxAmmo.ToString();

        weapon.onBulletChange -= UpdateCurrentAmmoDisplay;
        weapon.onBulletChange += UpdateCurrentAmmoDisplay;
    }


}
