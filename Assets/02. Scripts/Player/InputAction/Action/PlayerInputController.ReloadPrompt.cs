using UnityEngine;

public partial class PlayerInputController
{
    [Header("UI Prompt (Reload)")]
    [SerializeField] private GameObject rKeyPrompt;   // R키 애니 오브젝트(기본 비활성)

    [Tooltip("예비탄이 있을 때만 표시 (true 권장)")]
    [SerializeField] private bool showOnlyWhenReloadPossible = true;

    // --- 이벤트 핸들러 캐시 (구독/해제용) ---
    private void HandleWeaponChanged(Shooter _)
    {
        RefreshReloadPrompt();
    }
    private void HandleAmmoChanged()
    {
        RefreshReloadPrompt();
    }
    private void HandleReloaded()
    {
        RefreshReloadPrompt();
    }

    private void OnEnable()
    {
        var wm = WeaponManager.Instance;
        if (wm != null)
        {
            wm.OnWeaponChanged.AddListener(HandleWeaponChanged);
            wm.OnAmmoChanged.AddListener(HandleAmmoChanged);
            wm.OnReloaded.AddListener(HandleReloaded);
        }
        RefreshReloadPrompt();
    }

    private void OnDisable()
    {
        var wm = WeaponManager.Instance;
        if (wm != null)
        {
            wm.OnWeaponChanged.RemoveListener(HandleWeaponChanged);
            wm.OnAmmoChanged.RemoveListener(HandleAmmoChanged);
            wm.OnReloaded.RemoveListener(HandleReloaded);
        }
        if (rKeyPrompt != null) rKeyPrompt.SetActive(false);
    }

    private void Start()
    {
        // 씬 시작 시 한 번 반영
        RefreshReloadPrompt();
    }

    private void RefreshReloadPrompt()
    {
        if (rKeyPrompt == null)
            return;

        var wm = WeaponManager.Instance;
        if (wm == null || wm.CurrentWeapon == null)
        {
            rKeyPrompt.SetActive(false);
            return;
        }

        int mag = wm.GetMagazine(); // 현재 탄창
        int reserve = wm.GetReserve(); // 예비탄
        bool emptyMag = mag <= 0;

        bool show = emptyMag;
        if (showOnlyWhenReloadPossible)
            show &= (reserve > 0);   // 예비탄 없으면 숨김

        rKeyPrompt.SetActive(show);
    }
}
