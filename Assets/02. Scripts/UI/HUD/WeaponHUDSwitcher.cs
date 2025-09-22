using Constants;
using UnityEngine;

public class WeaponHUDSwitcher : MonoBehaviour
{
    [SerializeField] private UIWeaponHUD hud; // 실제 무기 아이콘, 탄약 표시 담당

    [Header("Icons")]
    [SerializeField] private Sprite pistolIcon; // 권총 상태 아이콘
    [SerializeField] private Sprite rifleIcon;  // 라이플 상태 아이콘

    // (선택) 인스펙터 숨기고 싶으면 [HideInInspector]
    private WeaponType activeType = WeaponType.Pistol;

    private void OnEnable()
    {
        // GamePhase 구독
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        // BulletManager 구독 (탄 수 변할 때마다 HUD 갱신)
        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.AddListener(RefreshAmmoUI);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;

        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.RemoveListener(RefreshAmmoUI);
    }

    private void Start()
    {
        // 시작은 권총 상태
        ApplyPistol();
    }

    private void HandlePhaseChanged(GamePhase phase)
    {
        if (phase == GamePhase.Stealth)
            ApplyPistol();
        else if (phase == GamePhase.Combat)
            ApplyRifle();
    }

    private void ApplyPistol()
    {
        activeType = WeaponType.Pistol;

        // BulletManager에도 현재 무기 타입을 알려줘서 소비/표시 일치
        if (BulletManager.Instance != null)
            BulletManager.Instance.SetCurrentWeapon(activeType);

        if (hud != null) hud.SetIcon(pistolIcon);
        RefreshAmmoUI();
    }

    private void ApplyRifle()
    {
        activeType = WeaponType.Rifle;

        if (BulletManager.Instance != null)
            BulletManager.Instance.SetCurrentWeapon(activeType);

        if (hud != null) hud.SetIcon(rifleIcon);
        RefreshAmmoUI();
    }

    private void RefreshAmmoUI()
    {
        if (hud == null || BulletManager.Instance == null) return;

        int mag = BulletManager.Instance.GetMagazine(activeType);
        int res = BulletManager.Instance.GetReserve(activeType);
        hud.SetAmmo(mag, res);
    }
}
