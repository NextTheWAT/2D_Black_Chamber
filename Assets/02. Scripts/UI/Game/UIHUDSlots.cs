using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHUDSlots : MonoBehaviour
{
    [Header("HP (ĭ ��Ȱ��ȭ ���)")]
    [SerializeField] private Transform hpGroup; // HPGroup (�ڽ� 7ĭ)
    [SerializeField] private TMP_Text hpLabel;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;

    [Header("Weapon Display (�ϳ��� ���̰�)")]
    [SerializeField] private GameObject pistolObject; //����
    [SerializeField] private GameObject rifleObject; //����
    [SerializeField] private GameObject knifeObject; //�Ѿ˾���
    [SerializeField] private TMP_Text ammoText; //�Ѿ� ��

    private readonly List<GameObject> _hpSlots = new();
    private Health _playerHealth;
    private PlayerConditionManager _staminaMgr;

    private void Awake()
    {
        // HP ĭ�� ����
        if (hpGroup)
        {
            _hpSlots.Clear();
            for (int i = 0; i < hpGroup.childCount; i++)
                _hpSlots.Add(hpGroup.GetChild(i).gameObject);
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player) _playerHealth = player.GetComponent<Health>();

        _staminaMgr = PlayerConditionManager.Instance;
    }

    private void OnEnable()
    {
        // ü�� �̺�Ʈ ����
        if (_playerHealth)
        {
            _playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            OnHealthChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }

        // ���¹̳� �̺�Ʈ ����
        if (_staminaMgr)
        {
            _staminaMgr.OnStamina01Changed += OnStaminaChanged01;
            OnStaminaChanged01(_staminaMgr.Stamina01);
        }

        // ����/ź�� ǥ�ô� �̺�Ʈ�� ������ �����ؼ� ����
        if (WeaponManager.Instance)
        {
            WeaponManager.Instance.OnWeaponChanged.AddListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.AddListener(OnAmmoChanged);
        }
        if (GameManager.Instance)
        {
            GameManager.Instance.OnPhaseChanged += _ => RefreshWeaponUI();
        }

        // �ʱ� ����
        RefreshWeaponUI();
        RefreshAmmo();
    }

    private void OnDisable()
    {
        if (_playerHealth)
            _playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);

        if (_staminaMgr)
            _staminaMgr.OnStamina01Changed -= OnStaminaChanged01;

        if (WeaponManager.Instance)
        {
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.RemoveListener(OnAmmoChanged);
        }

        if (GameManager.Instance)
            GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnHealthChanged(int cur, int max)
    {
        if (_hpSlots.Count > 0)
        {
            float ratio = max > 0 ? (float)cur / max : 0f;
            int onCount = Mathf.CeilToInt(ratio * _hpSlots.Count);    // ��: 80% > 6ĭ
            for (int i = 0; i < _hpSlots.Count; i++)
                _hpSlots[i].SetActive(i < onCount);
        }
        if (hpLabel) hpLabel.text = $"{cur}/{max}"; //ü�� ���� ǥ�ÿ� ���
    }

    private void OnStaminaChanged01(float v01)
    {
        if (staminaBar) staminaBar.fillAmount = Mathf.Clamp01(v01);
    }

    private void OnWeaponChanged(Shooter _) //���� ����
    {
        RefreshWeaponUI();
        RefreshAmmo();
    }

    private void OnAmmoChanged() //ź�� �ؽ�Ʈ ����
    {
        RefreshWeaponUI();
    }

    private void OnPhaseChanged(Constants.GamePhase _) //�� ������ ��ȯ
    {
        RefreshWeaponUI();
    }

    private void RefreshWeaponUI()
    {
        if (WeaponManager.Instance == null || WeaponManager.Instance.CurrentWeapon == null)
            return;

        // �Ѿ��� �ϳ��� ������ Knife ǥ��
        int total = WeaponManager.Instance.GetMagazine() + WeaponManager.Instance.GetReserve();
        bool hasAnyAmmo = total > 0;

        if (!hasAnyAmmo)
        {
            ShowOnly(knifeObject);
            if (ammoText) ammoText.text = "--";
            return;
        }

        // ���� ���¸� Rifle, ���� ���¸� Pistol
        bool isCombat = GameManager.Instance &&
                        GameManager.Instance.CurrentPhase == Constants.GamePhase.Combat;
        ShowOnly(isCombat ? rifleObject : pistolObject);

        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        if (!ammoText || WeaponManager.Instance == null || WeaponManager.Instance.CurrentWeapon == null) 
            return;

        int mag = WeaponManager.Instance.GetMagazine();
        int res = WeaponManager.Instance.GetReserve();
        ammoText.text = $"{res} / {mag}";
    }

    private void ShowOnly(GameObject go)
    {
        if (pistolObject) pistolObject.SetActive(go == pistolObject);
        if (rifleObject) rifleObject.SetActive(go == rifleObject);
        if (knifeObject) knifeObject.SetActive(go == knifeObject);
    }
}
