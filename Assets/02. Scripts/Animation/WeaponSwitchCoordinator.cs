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
        }
    }

    private void OnDisable()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(OnWeaponChanged);
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
        if (shooter == null || shooter.gunData == null) return;

        // 1) 상체 애니메이터 컨트롤러 스왑
        anim.ApplyUpperWeaponAnimator(shooter.gunData, playSwitchAnim: true);
    }
}
