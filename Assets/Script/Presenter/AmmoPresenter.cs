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

    // 현재 총알 개수 UI 변경 함수
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
        // 현재무기가 있을경우
        if(currentWeapon != null)
        {
            // viewer관련 델리게이트 삭제
            currentWeapon.DeactivateAmmoDelegate();
        }
        
        // viewer 와 현재무기 델리게이트 연결
        currentWeapon = weapon;
        currentWeapon.onReloadTimeChange += UpdateIconDisplay;
        currentWeapon.onCurrentBulletChange += UpdateCurrentAmmoDisplay;
        currentWeapon.onTotalBulletChange += UpdateTotalAmmoDisplay;

        // 무기 UI 한 번 갱신
        UpdateCurrentAmmoDisplay(weapon.CurrentAmmo);
        UpdateTotalAmmoDisplay(weapon.TotalAmmo);
    }
}
