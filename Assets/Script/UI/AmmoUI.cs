using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    TextMeshProUGUI m_TotalAmmoGUI;
    TextMeshProUGUI m_CurrentAmmoGUI;
    TextMeshProUGUI m_GrenadeCountGUI;

    Image m_AmmoIcon;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        m_AmmoIcon = child.GetComponent<Image>();

        child = transform.GetChild(1);
        m_TotalAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        m_CurrentAmmoGUI = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(4);
        m_GrenadeCountGUI = child.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        PlayerMovementContoller player = GameManager.Instance.Player;

        player.onWeaponChange += UpdateAmmoDisplay;

        foreach(var weapon in player.GetComponentsInChildren<Weapon>(true))
        {
            weapon.onCurrentBulletChange += UpdateCurrentAmmoDisplay;
            weapon.onReloadTimeChange += UpdateAmmoIconDisplay;   // ������ �� ������ filled �� ��ȭ
            weapon.onTotalBulletChange += UpdateTotalAmmoDisplay; // pickup ���� ���� �� ������ ���Ұ�� ����ȴ�
        }

        // ��ô���� ���� �̺�Ʈ ����
        player.GrenadeHandler.onGrenadeCountChange += UpdateGrenadeCountDisplay;
        // player �� setWeapon �Լ��� ������ �ڿ� UpdateMaxAmmoDisplay �Լ��� ��ϵȴ�. ���� Ÿ�̹��� �ȸ±� ������ Update�� �ѹ� ���ش�.
        // UpdateAmmoDisplay(player.CurrentWeapon);
        // UpdateCurrentAmmoDisplay(player.CurrentWeapon.maxAmmo);
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
        // ���� �÷��̾ ������ �ִ� ���� �Ѿ��� �ѷ��� capacity�� ���� ���� UI�� ������Ʈ�Ѵ�.
        // ���� ������� ���� �ƴ� �ٸ����� �Ⱦ� �������� �Ծ��� �� UI �� ����Ǵ°� ������.
        if(GameManager.Instance.Player.CurrentWeapon.TotalAmmo == capacity)
        {
            m_TotalAmmoGUI.text = capacity.ToString();
        }
    }

    private void UpdateAmmoIconDisplay(float ratio)
    {
        m_AmmoIcon.fillAmount = ratio;
    }

    private void UpdateGrenadeCountDisplay(int count)
    {
        m_GrenadeCountGUI.text = count.ToString();
    }
}
