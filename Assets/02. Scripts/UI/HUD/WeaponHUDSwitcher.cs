using Constants;
using UnityEngine;

public class WeaponHUDSwitcher : MonoBehaviour
{
    [SerializeField] private UIWeaponHUD hud; // ���� ���� ������, ź�� ǥ�� ���

    [Header("Icons")]
    [SerializeField] private Sprite pistolIcon; // ���� ���� ������
    [SerializeField] private Sprite rifleIcon;  // ������ ���� ������

    // (����) �ν����� ����� ������ [HideInInspector]
    private WeaponType activeType = WeaponType.Pistol;

    private void OnEnable()
    {
        // GamePhase ����
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        // BulletManager ���� (ź �� ���� ������ HUD ����)
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
        // ������ ���� ����
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

        // BulletManager���� ���� ���� Ÿ���� �˷��༭ �Һ�/ǥ�� ��ġ
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
