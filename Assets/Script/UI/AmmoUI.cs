using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    TextMeshProUGUI m_MaxAmmoGUI;
    TextMeshProUGUI m_CurrentAmmoGUI;

    Image m_AmmoIcon;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        m_AmmoIcon = child.GetComponent<Image>();

        child = transform.GetChild(1);
        m_MaxAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        m_CurrentAmmoGUI = child.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Player player = GameManager.Instance.Player;

        player.onWeaponChange += UpdateAmmoDisplay;

        foreach(var weapon in player.GetComponentsInChildren<Weapon>(true))
        {
            weapon.onBulletChange += UpdateCurrentAmmoDisplay;
            weapon.onReloadTimeChange += UpdateAmmoIconDisplay;
        }

        // player �� setWeapon �Լ��� ������ �ڿ� UpdateMaxAmmoDisplay �Լ��� ��ϵȴ�. ���� Ÿ�̹��� �ȸ±� ������ Update�� �ѹ� ���ش�.
        UpdateAmmoDisplay(player.CurrentWeapon);
        UpdateCurrentAmmoDisplay(player.CurrentWeapon.maxAmmo);
    }

    public void UpdateCurrentAmmoDisplay(int count)
    {
        m_CurrentAmmoGUI.text = count.ToString();
    }

    private void UpdateAmmoDisplay(Weapon weapon)
    {
        m_MaxAmmoGUI.text = weapon.maxAmmo.ToString();
        m_CurrentAmmoGUI.text = weapon.CurrentAmmo.ToString();

        // Weapon �� ��������Ʈ�� ����
        //weapon.onBulletChange -= UpdateCurrentAmmoDisplay;
        //weapon.onBulletChange += UpdateCurrentAmmoDisplay;

        //weapon.onReloadTimeChange -= UpdateAmmoIconDisplay;
        //weapon.onReloadTimeChange += UpdateAmmoIconDisplay;
    }

    private void UpdateAmmoIconDisplay(float ratio)
    {
        m_AmmoIcon.fillAmount = ratio;
    }
}
