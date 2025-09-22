// WeaponHUDSwitcher.cs (전체 교체)
using Constants;
using UnityEngine;

public class WeaponHUDSwitcher : MonoBehaviour
{
    // [SerializeField] private UIWeaponHUD hud;

    private void OnEnable()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponChanged.AddListener(RefreshAll);
            WeaponManager.Instance.OnAmmoChanged.AddListener(RefreshAmmo);
        }

        // 초기 1회 갱신
        RefreshAll(WeaponManager.Instance.CurrentWeapon);
    }

    private void OnDisable()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(RefreshAll);
            WeaponManager.Instance.OnAmmoChanged.RemoveListener(RefreshAmmo);
        }
    }

    private void RefreshAll(GunData gunData)
    {
        /*
        if (!hud) return;
        hud.SetIcon(gunData.weaponSprite);
        RefreshAmmo();
        */
    }

    private void RefreshAmmo()
    {
        /*
        if (!hud || WeaponManager.Instance == null) return;
        hud.SetAmmo(WeaponManager.Instance.GetMagazine(), WeaponManager.Instance.GetReserve());
        */
    }
}
