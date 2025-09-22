using UnityEngine;
using Constants;

public class WeaponSwitchCoordinator : MonoBehaviour
{
    [SerializeField] private CharacterAnimationController anim;
    [SerializeField] private UIWeaponHUD hud; // ����: ���⼭ ź ǥ�ñ��� �����Ϸ��� ���

    private void Awake()
    {
        if (!anim) anim = GetComponent<CharacterAnimationController>();
        if (!hud) hud = UIWeaponHUD.Instance; // ����
    }

    private void OnEnable()
    {
        if (WeaponManager.Instance != null)
            WeaponManager.Instance.OnWeaponChanged.AddListener(OnWeaponChanged);

        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.AddListener(RefreshAmmo);
    }

    private void OnDisable()
    {
        if (WeaponManager.Instance != null)
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(OnWeaponChanged);

        if (BulletManager.Instance != null)
            BulletManager.Instance.OnAmmoChanged.RemoveListener(RefreshAmmo);
    }

    private void Start()
    {
        // ���� ���� �ݿ�
        if (WeaponManager.Instance != null)
            OnWeaponChanged(WeaponManager.Instance.CurrentWeapon);
        else
            OnWeaponChanged(WeaponType.Pistol);
    }

    private void OnWeaponChanged(WeaponType type)
    {
        // 1) ��ü �ִϸ����� ��Ʈ�ѷ� ����
        anim?.ApplyUpperWeaponAnimator(type, playSwitchAnim: true);

        // 2) (����) ź ǥ�ô� HUD ��ũ��Ʈ���� ó�� ���̸� ���� ����
        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        if (!hud || WeaponManager.Instance == null || BulletManager.Instance == null) return;
        var type = WeaponManager.Instance.CurrentWeapon;
        hud.SetAmmo(BulletManager.Instance.GetMagazine(type), BulletManager.Instance.GetReserve(type));
    }
}
