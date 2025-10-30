using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayerUI : MonoBehaviour
{
    [Header("Weapon Display")]
    [SerializeField] private GameObject pistolObject; //잠입
    [SerializeField] private GameObject rifleObject; //난전
    [SerializeField] private GameObject knifeObject; //총알없음

    [SerializeField] private Image pistolImage;
    [SerializeField] private Image rifleImage;
    [SerializeField] private Image knifeImage;

    [SerializeField] private TMP_Text ammoText; //총알 수

    [Header("Weapon Scale (선택/비선택)")]
    [SerializeField] private Vector3 selectedScale = new Vector3(1.15f, 1.15f, 1f);
    [SerializeField] private Vector3 deselectedScale = new Vector3(0.9f, 0.9f, 1f);

    [Header("Money Display")]
    [SerializeField] private TMP_Text moneyText;

    private void Awake()
    {
        if(MoneyManager.Instance != null)
        {
            RefreshMoney();
        }
    }

    private void OnEnable()
    {
        // 머니 이벤트 구독
        MoneyManager.Instance.OnMoneyChanged.AddListener(RefreshMoney);


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

        if (MoneyManager.Instance) MoneyManager.Instance.OnMoneyChanged.RemoveListener(RefreshMoney);


        if (WeaponManager.Instance)
        {
            WeaponManager.Instance.OnWeaponChanged.RemoveListener(OnWeaponChanged);
            WeaponManager.Instance.OnAmmoChanged.RemoveListener(OnAmmoChanged);
        }

        if (GameManager.Instance)
            GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }
    private void RefreshMoney()
    {
        if (moneyText) moneyText.text = $"$ {MoneyManager.Instance.Balance:N0}";
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
        WeaponManager weaponManager = WeaponManager.Instance;
        if (weaponManager == null || weaponManager.CurrentWeapon == null)
            return;

        int total = weaponManager.GetMagazine() + weaponManager.GetReserve();
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

        if (pistolImage && weaponManager.StealthWeapon)
            pistolImage.sprite = weaponManager.StealthWeapon.gunData.prefabInfo.weaponSprite;

        if (rifleImage && weaponManager.CombatWeapon)
            rifleImage.sprite = weaponManager.CombatWeapon.gunData.prefabInfo.weaponSprite;

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

    private void SetWeaponScale(GameObject weaponObj, bool isActive)
    {
        if (!weaponObj) return;

        // UI 요소이면 RectTransform, 아니면 일반 Transform에 적용
        var rt = weaponObj.GetComponent<RectTransform>();
        if (rt != null)
            rt.localScale = isActive ? selectedScale : deselectedScale;
        else
            weaponObj.transform.localScale = isActive ? selectedScale : deselectedScale;
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

        // 각 무기 스케일 설정 (선택된 것은 크게, 나머지는 작게)
        SetWeaponScale(pistolObject, pistolObject == activeWeapon);
        SetWeaponScale(rifleObject, rifleObject == activeWeapon);
        SetWeaponScale(knifeObject, knifeObject == activeWeapon);
    }


}
