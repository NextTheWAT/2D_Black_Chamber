using Constants;
using UnityEngine;

public class UIWeaponHUDGamePhaseConnector : MonoBehaviour
{
    [SerializeField] private UIWeaponHUD hud; //���� ���� ������, ź�� ǥ�� ���

    [Header("Icons")]
    [SerializeField] private Sprite pistolIcon; //���� ���� ������
    [SerializeField] private Sprite rifleIcon; //������ ���� ������

    [Header("Ammo Settings")]
    [SerializeField] private int pistolMagazineSize = 6; //���� ��ź��
    [SerializeField] private int rifleMagazineSize = 40; //������ ��ź��
    [SerializeField] private int reserveAmmo = 120; //���� ����ź

    private int currentMagazine; //���� źâ�� ���� ��

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
        currentMagazine = pistolMagazineSize; // ���� ��ź ���� ä��
        hud.SetIcon(pistolIcon);
        hud.SetAmmo(currentMagazine, reserveAmmo);
    }

    private void ApplyRifle()
    {
        currentMagazine = rifleMagazineSize; // ������ ��ź ���� ä��
        hud.SetIcon(rifleIcon);
        hud.SetAmmo(currentMagazine, reserveAmmo);
    }
}
