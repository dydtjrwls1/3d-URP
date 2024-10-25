using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    TextMeshProUGUI m_TotalAmmoGUI;
    TextMeshProUGUI m_CurrentAmmoGUI;

    Image m_AmmoIcon;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        m_AmmoIcon = child.GetComponent<Image>();

        child = transform.GetChild(1);
        m_TotalAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        m_CurrentAmmoGUI = child.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Player player = GameManager.Instance.Player;

        player.onWeaponChange += UpdateAmmoDisplay;

        foreach(var weapon in player.GetComponentsInChildren<Weapon>(true))
        {
            weapon.onCurrentBulletChange += UpdateCurrentAmmoDisplay;
            weapon.onReloadTimeChange += UpdateAmmoIconDisplay;   // 재장전 시 아이콘 filled 를 변화
            weapon.onTotalBulletChange += UpdateTotalAmmoDisplay; // pickup 으로 인해 총 개수가 변할경우 실행된다
        }

        // player 의 setWeapon 함수가 시작한 뒤에 UpdateMaxAmmoDisplay 함수가 등록된다. 실행 타이밍이 안맞기 때문에 Update를 한번 해준다.
        UpdateAmmoDisplay(player.CurrentWeapon);
        UpdateCurrentAmmoDisplay(player.CurrentWeapon.maxAmmo);
    }

    public void UpdateCurrentAmmoDisplay(int count)
    {
        m_CurrentAmmoGUI.text = count.ToString();
    }

    private void UpdateAmmoDisplay(Weapon weapon)
    {
        m_TotalAmmoGUI.text = weapon.TotalAmmo.ToString();
        m_CurrentAmmoGUI.text = weapon.CurrentAmmo.ToString();
    }

    private void UpdateTotalAmmoDisplay(int capacity)
    {
        m_TotalAmmoGUI.text = capacity.ToString();
    }

    private void UpdateAmmoIconDisplay(float ratio)
    {
        m_AmmoIcon.fillAmount = ratio;
    }
}
