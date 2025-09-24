using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHUDSlots : MonoBehaviour
{
    [Header("HP (칸 비활성화 방식)")]
    [SerializeField] private Transform hpGroup; // HPGroup (자식 7칸)
    [SerializeField] private TMP_Text hpLabel;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;

    [Header("Weapon Display (하나만 보이게)")]
    [SerializeField] private GameObject pistolObject; //잠입
    [SerializeField] private GameObject rifleObject; //난전
    [SerializeField] private GameObject knifeObject; //총알없음
    [SerializeField] private TMP_Text ammoText; //총알 수

    private readonly List<GameObject> _hpSlots = new();
    private Health _playerHealth;
    private PlayerConditionManager _staminaMgr;

    private void Awake()
    {
        // HP 칸들 수집
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
        // 체력 이벤트 구독
        if (_playerHealth)
        {
            _playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            OnHealthChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }

        // 스태미나 이벤트 구독
        if (_staminaMgr)
        {
            _staminaMgr.OnStamina01Changed += OnStaminaChanged01;
            OnStaminaChanged01(_staminaMgr.Stamina01);
        }

        // 무기/탄약 표시는 이벤트가 있으면 구독해서 갱신
        if (WeaponManager.Instance)
        {
            WeaponManager.Instance.OnWeaponChanged.AddListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.AddListener(OnAmmoChanged);
        }
        if (GameManager.Instance)
        {
            GameManager.Instance.OnPhaseChanged += _ => RefreshWeaponUI();
        }

        // 초기 갱신
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
            int onCount = Mathf.CeilToInt(ratio * _hpSlots.Count);    // 예: 80% > 6칸
            for (int i = 0; i < _hpSlots.Count; i++)
                _hpSlots[i].SetActive(i < onCount);
        }
        if (hpLabel) hpLabel.text = $"{cur}/{max}"; //체력 숫자 표시에 사용
    }

    private void OnStaminaChanged01(float v01)
    {
        if (staminaBar) staminaBar.fillAmount = Mathf.Clamp01(v01);
    }

    private void OnWeaponChanged(Shooter _) //무기 변경
    {
        RefreshWeaponUI();
        RefreshAmmo();
    }

    private void OnAmmoChanged() //탄약 텍스트 갱신
    {
        RefreshWeaponUI();
    }

    private void OnPhaseChanged(Constants.GamePhase _) //총 아이콘 전환
    {
        RefreshWeaponUI();
    }

    private void RefreshWeaponUI()
    {
        if (WeaponManager.Instance == null || WeaponManager.Instance.CurrentWeapon == null)
            return;

        // 총알이 하나도 없으면 Knife 표시
        int total = WeaponManager.Instance.GetMagazine() + WeaponManager.Instance.GetReserve();
        bool hasAnyAmmo = total > 0;

        if (!hasAnyAmmo)
        {
            ShowOnly(knifeObject);
            if (ammoText) ammoText.text = "--";
            return;
        }

        // 전투 상태면 Rifle, 잠입 상태면 Pistol
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
