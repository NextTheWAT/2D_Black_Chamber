using Constants;
using UnityEngine;

public class UIWeaponHUDGamePhaseConnector : MonoBehaviour
{
    [SerializeField] private UIWeaponHUD hud; //실제 무기 아이콘, 탄약 표시 담당

    [Header("Icons")]
    [SerializeField] private Sprite pistolIcon; //권총 상태 아이콘
    [SerializeField] private Sprite rifleIcon; //라이플 상태 아이콘

    [Header("Ammo Settings")]
    [SerializeField] private int pistolMagazineSize = 6; //권총 장탄수
    [SerializeField] private int rifleMagazineSize = 40; //라이플 장탄수
    [SerializeField] private int reserveAmmo = 120; //공용 예비탄

    private int currentMagazine; //현재 탄창에 남은 수

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
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
        currentMagazine = pistolMagazineSize; // 권총 장탄 가득 채움
        hud.SetIcon(pistolIcon);
        hud.SetAmmo(currentMagazine, reserveAmmo);
    }

    private void ApplyRifle()
    {
        currentMagazine = rifleMagazineSize; // 라이플 장탄 가득 채움
        hud.SetIcon(rifleIcon);
        hud.SetAmmo(currentMagazine, reserveAmmo);
    }
}
