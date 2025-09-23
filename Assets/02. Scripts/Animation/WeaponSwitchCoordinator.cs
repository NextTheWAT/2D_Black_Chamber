using UnityEngine;
using Constants;

public class WeaponSwitchCoordinator : MonoBehaviour
{
    [SerializeField] private CharacterAnimationController anim;
    // [SerializeField] private UIWeaponHUD hud; // ����: ���⼭ ź ǥ�ñ��� �����Ϸ��� ���

    private void Awake()
    {
        if (!anim) anim = GetComponent<CharacterAnimationController>();
        // if (!hud) hud = UIWeaponHUD.Instance; // ����
    }

    private void OnEnable()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponChanged.AddListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.AddListener(RefreshAmmo);
        }
    }

    private void OnDisable()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.RemoveListener(RefreshAmmo);
        }

    }

    private void Start()
    {
        // ���� ���� �ݿ�
        if (WeaponManager.Instance != null)
            OnWeaponChanged(WeaponManager.Instance.CurrentWeapon);
    }

    private void OnWeaponChanged(Shooter shooter)
    {
        // 1) ��ü �ִϸ����� ��Ʈ�ѷ� ����
        anim?.ApplyUpperWeaponAnimator(shooter.gunData, playSwitchAnim: true);

        // 2) (����) ź ǥ�ô� HUD ��ũ��Ʈ���� ó�� ���̸� ���� ����
        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        /*
        if (!hud || WeaponManager.Instance == null) return;
        hud.SetAmmo(WeaponManager.Instance.GetMagazine(), WeaponManager.Instance.GetReserve());
        */
    }
}
