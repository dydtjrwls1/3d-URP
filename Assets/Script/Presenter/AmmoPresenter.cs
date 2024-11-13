using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPresenter : MonoBehaviour
{
    [SerializeField]
    PlayerWeaponHandler handler;

    [SerializeField]
    AmmoViewer viewer;

    Weapon currentWeapon;

    private void Awake()
    {
        handler.onWeaponChange += OnWeaponChange;
        handler.onGrenadeCountChange += UpdateGrenadeCountDisplay;
    }

    private void UpdateGrenadeCountDisplay(int count)
    {
        viewer.GrenadeUI.text = count.ToString();
    }

    // ���� �Ѿ� ���� UI ���� �Լ�
    private void UpdateCurrentAmmoDisplay(int ammo)
    {
        viewer.CurrentAmmoGUI.text = ammo.ToString();
    }

    private void UpdateTotalAmmoDisplay(int ammo)
    {
        viewer.TotalAmmoUI.text = ammo.ToString();
    }

    private void UpdateIconDisplay(float ratio)
    {
        viewer.Icon.fillAmount = ratio;
    }

    private void OnWeaponChange(Weapon weapon)
    {
        // ���繫�Ⱑ �������
        if(currentWeapon != null)
        {
            // viewer���� ��������Ʈ ����
            currentWeapon.DeactivateAmmoDelegate();
        }
        
        // viewer �� ���繫�� ��������Ʈ ����
        currentWeapon = weapon;
        currentWeapon.onReloadTimeChange += UpdateIconDisplay;
        currentWeapon.onCurrentBulletChange += UpdateCurrentAmmoDisplay;
        currentWeapon.onTotalBulletChange += UpdateTotalAmmoDisplay;

        // ���� UI �� �� ����
        UpdateCurrentAmmoDisplay(weapon.CurrentAmmo);
        UpdateTotalAmmoDisplay(weapon.TotalAmmo);
    }
}
