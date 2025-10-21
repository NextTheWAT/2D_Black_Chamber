using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHUDSlots : MonoBehaviour
{
    [Header("HP (줄어드는 방식)")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpLabel;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text staminaLabel;

    [Header("Weapon Display")]
    [SerializeField] private GameObject pistolObject; //잠입
    [SerializeField] private GameObject rifleObject; //난전
    [SerializeField] private GameObject knifeObject; //총알없음
    [SerializeField] private TMP_Text ammoText; //총알 수

    private readonly List<GameObject> _hpSlots = new();
    private Health _playerHealth;
    private PlayerConditionManager _staminaMgr;

    private void Awake()
    {
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
        float ratio = max > 0 ? (float)cur / max : 0f;

        // HP 이미지 바에 반영
        if (hpBar) hpBar.fillAmount = Mathf.Clamp01(ratio);

        // 숫자 텍스트도 유지
        if (hpLabel) hpLabel.text = $"{cur}";
    }

    private void OnStaminaChanged01(float v01)
    {
        if (staminaBar) staminaBar.fillAmount = Mathf.Clamp01(v01);

        if (staminaLabel && _staminaMgr != null)
        {
            int cur = Mathf.RoundToInt(_staminaMgr.CurrentStamina);
            staminaLabel.text = $"{cur}";
        }
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

        int total = WeaponManager.Instance.GetMagazine() + WeaponManager.Instance.GetReserve();
        bool hasAnyAmmo = total > 0;

        GameObject activeObj;

        if (!hasAnyAmmo)
        {
            activeObj = knifeObject;
            if (ammoText) ammoText.text = "--";
        }
        else
        {
            bool isCombat = GameManager.Instance &&
                            GameManager.Instance.CurrentPhase == Constants.GamePhase.Combat;
            activeObj = isCombat ? rifleObject : pistolObject;
        }

        UpdateWeaponVisuals(activeObj);
        RefreshAmmo();
    }


    private void RefreshAmmo()
    {
        if (!ammoText || WeaponManager.Instance == null || WeaponManager.Instance.CurrentWeapon == null) 
            return;

        int mag = WeaponManager.Instance.GetMagazine();
        int res = WeaponManager.Instance.GetReserve();
        ammoText.text = $"{mag} / {res}";
    }

    private void ShowOnly(GameObject go)
    {
        if (pistolObject) pistolObject.SetActive(go == pistolObject);
        if (rifleObject) rifleObject.SetActive(go == rifleObject);
        if (knifeObject) knifeObject.SetActive(go == knifeObject);
    }

    private void SetWeaponColor(GameObject weaponObj, bool isActive)
    {
        if (!weaponObj) return;

        // 자식 중 이름이 Background 또는 Icon인 Image 컴포넌트들 가져오기
        var images = weaponObj.GetComponentsInChildren<Image>(true);

        foreach (var img in images)
        {
            img.color = isActive ? Color.white : new Color(0.3f, 0.3f, 0.3f); // 회색 처리
        }
    }

    private void UpdateWeaponVisuals(GameObject activeWeapon)
    {
        // 모든 무기 오브젝트 활성화
        if (pistolObject) pistolObject.SetActive(true);
        if (rifleObject) rifleObject.SetActive(true);
        if (knifeObject) knifeObject.SetActive(true);

        // 각 무기 색상 설정
        SetWeaponColor(pistolObject, pistolObject == activeWeapon);
        SetWeaponColor(rifleObject, rifleObject == activeWeapon);
        SetWeaponColor(knifeObject, knifeObject == activeWeapon);
    }


}
