using UnityEngine;
using Constants;

public class WeaponSwitchCoordinator : MonoBehaviour
{
    [SerializeField] private CharacterAnimationController anim;
    // [SerializeField] private UIWeaponHUD hud; // 선택: 여기서 탄 표시까지 갱신하려면 사용

    private void Awake()
    {
        if (!anim) anim = GetComponent<CharacterAnimationController>();
        // if (!hud) hud = UIWeaponHUD.Instance; // 선택
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
        // 시작 상태 반영
        if (WeaponManager.Instance != null)
            OnWeaponChanged(WeaponManager.Instance.CurrentWeapon);
    }

    private void OnWeaponChanged(Shooter shooter)
    {
        // 1) 상체 애니메이터 컨트롤러 스왑
        anim?.ApplyUpperWeaponAnimator(shooter.gunData, playSwitchAnim: true);

        // 2) (선택) 탄 표시는 HUD 스크립트에서 처리 중이면 생략 가능
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
