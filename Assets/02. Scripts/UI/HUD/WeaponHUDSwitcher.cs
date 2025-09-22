// WeaponHUDSwitcher.cs (전체 교체)
using Constants;
using UnityEngine;

public class WeaponHUDSwitcher : MonoBehaviour
{
    [SerializeField] private UIWeaponHUD hud;
    [SerializeField] private Sprite pistolIcon;
    [SerializeField] private Sprite rifleIcon;

    private void OnEnable()
    {
        if (WeaponManager.Instance != null)
            WeaponManager.Instance.OnWeaponChanged.AddListener(RefreshAll);

        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.AddListener(RefreshAmmo);

        // 초기 1회 갱신
        RefreshAll(WeaponManager.Instance ? WeaponManager.Instance.CurrentWeapon : WeaponType.Pistol);
    }

    private void OnDisable()
    {
        if (WeaponManager.Instance != null)
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(RefreshAll);

        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.RemoveListener(RefreshAmmo);
    }

    private void RefreshAll(WeaponType type)
    {
        if (!hud) return;
        hud.SetIcon(type == WeaponType.Rifle ? rifleIcon : pistolIcon);
        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        if (!hud || WeaponManager.Instance == null || BulletManager.Instance == null) return;
        var type = WeaponManager.Instance.CurrentWeapon;
        hud.SetAmmo(BulletManager.Instance.GetMagazine(type), BulletManager.Instance.GetReserve(type));
    }
}
