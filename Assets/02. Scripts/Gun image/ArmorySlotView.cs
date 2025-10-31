using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants; // GamePhase
using DG.Tweening;

/// <summary>
/// 무기방의 "한 슬롯(잠입/난전)"을 담당하는 초간단 컨트롤러.
/// - 타입(HG/AR 등) 필터 없음.
/// - Owned 전체를 돌려가며 선택 저장.
/// - 옵션: 페이즈 태그(Any/Stealth/Combat)로만 걸러보기.
/// </summary>
public class ArmorySlotViewSimple : MonoBehaviour
{
    public enum SlotPhase { Stealth, Combat }

    [Header("Which slot? (Stealth / Combat)")]
    public SlotPhase slot = SlotPhase.Stealth;

    [Header("Filter Option")]
    [Tooltip("체크하면 해당 페이즈(+Any)만 후보에 포함. 끄면 Owned 전부 대상.")]
    public bool filterByPhaseTag = true;

    [Header("UI")]
    public Gunimage carousel;    // 기존 캐러셀
    public Button nextButton;    // 오른쪽 화살표
    public Button prevButton;    // 왼쪽 화살표

    [Header("Bottom Texts")]
    public TMP_Text titleText;   // 무기 이름
    public TMP_Text damageText;  // 공격력
    public TMP_Text accuracyText;// 명중률(정확도)
    public TMP_Text rpmText;     // 발사 속도
    public TMP_Text ammoText;    // 장탄수

    // 내부 상태
    private List<GunData> candidates = new List<GunData>();
    private int currentItemIndex = 0;

    private void Awake()
    {
        if (nextButton) nextButton.onClick.AddListener(() => Step(+1));
        if (prevButton) prevButton.onClick.AddListener(() => Step(-1));
    }

    private void OnEnable()
    {
        BuildCandidates();
        PickInitialFromLoadout();
        PaintPanelsWithCurrent();
        RefreshDetails();
    }

    // ----- 후보 구성: Owned 전체(옵션: 페이즈 태그로만 필터) -----
    private void BuildCandidates()
    {
        candidates.Clear();

        var inv = WeaponInventory.Instance;
        if (inv == null || inv.Owned == null) return;

        IEnumerable<GunData> q = inv.Owned.Where(d => d != null);

        if (filterByPhaseTag)
        {
            var want = (slot == SlotPhase.Stealth) ? GunData.PhaseTag.Stealth : GunData.PhaseTag.Combat;
            q = q.Where(d => d.prefabInfo.phaseTag == want || d.prefabInfo.phaseTag == GunData.PhaseTag.Any);
        }

        // 스프라이트 없어도 동작은 하지만, UI 품질을 위해 있으면 우선
        // (스프라이트 없는 것도 포함하려면 아래 Where 제거)
        q = q.Where(d => d.prefabInfo.weaponSprite != null);

        candidates = q.Distinct().ToList();
        currentItemIndex = Mathf.Clamp(currentItemIndex, 0, Mathf.Max(0, candidates.Count - 1));
    }

    // ----- 저장된 로드아웃을 현재 선택으로 -----
    private void PickInitialFromLoadout()
    {
        if (candidates.Count == 0) return;

        string savedId = (slot == SlotPhase.Stealth)
            ? LoadoutProfile.Instance?.GetStealthId()
            : LoadoutProfile.Instance?.GetCombatId();

        if (string.IsNullOrEmpty(savedId)) { currentItemIndex = 0; return; }

        var savedData = WeaponInventory.Instance.FindById(savedId);
        int idx = candidates.FindIndex(d => d == savedData);
        currentItemIndex = (idx >= 0) ? idx : 0;
    }

    // ----- 캐러셀 패널 이미지를 "현재 선택" 스프라이트로 칠해두기 -----
    private void PaintPanelsWithCurrent()
    {
        if (!carousel || carousel.panels == null) return;
        var cur = GetCurrent();
        var sp = cur != null ? cur.prefabInfo.weaponSprite : null;

        for (int i = 0; i < carousel.panels.Length; i++)
        {
            var img = carousel.panels[i].GetComponent<Image>();
            if (!img) continue;

            img.sprite = sp;

            // 스프라이트 교체 시 정렬/위치 보정(위로 붙는 증상 방지)
            var r = img.rectTransform;
            r.anchorMin = new Vector2(0.5f, 0.5f);
            r.anchorMax = new Vector2(0.5f, 0.5f);
            r.pivot = new Vector2(0.5f, 0.5f);
            r.anchoredPosition = Vector2.zero;

            // 필요 시, 비율 유지
            img.preserveAspect = true;
            // img.SetNativeSize(); // 디자인에 맞게 선택
        }
    }

    // ----- 좌우 화살표 처리 -----
    private void Step(int delta)
    {
        if (candidates == null || candidates.Count == 0 || carousel == null) return;

        // 다음 인덱스(순환)
        int next = currentItemIndex + delta;
        if (next >= candidates.Count) next = 0;
        else if (next < 0) next = candidates.Count - 1;

        // 현재/다음 스프라이트
        var curSprite = candidates[currentItemIndex]?.prefabInfo?.weaponSprite;
        var nextSprite = candidates[next]?.prefabInfo?.weaponSprite;

        // 슬라이드 전에 from=현재, to=다음 스프라이트 확정하고 슬라이드
        carousel.SlideWithSprites(
            toRight: delta > 0,
            fromSprite: curSprite,
            toSprite: nextSprite,
            onComplete: () =>
            {
                // 슬라이드가 끝난 뒤에만 확정/장착/통일
                currentItemIndex = next;
                RefreshDetails();             // (있다면) 스펙 텍스트 갱신
                SaveLoadoutAndApply();        // (있다면) 실제 장착 반영
                carousel.SetAllSprites(nextSprite); // 패널 전체 통일
            }
        );
    }

    // ----- 하단 스펙 갱신 -----
    private void RefreshDetails()
    {
        var d = GetCurrent();
        if (d == null)
        {
            if (titleText) titleText.text = "-";
            if (damageText) damageText.text = "공격력 : -";
            if (accuracyText) accuracyText.text = "명중률 : -";
            if (rpmText) rpmText.text = "발사 속도 : -";
            if (ammoText) ammoText.text = "장탄수 : -";
            return;
        }

        string nameToShow = !string.IsNullOrEmpty(d.weaponName)
            ? d.weaponName
            : WeaponCatalog.GetDisplayName(d);

        if (titleText) titleText.text = nameToShow;
        if (damageText) damageText.text = $"공격력 : {d.damage}";
        if (accuracyText) accuracyText.text = $"명중률 : {Mathf.RoundToInt(d.accuracy)}";
        if (rpmText) rpmText.text = $"발사 속도 : {d.rpm}";
        if (ammoText) ammoText.text = $"장탄수 : {d.maxAmmo}";
    }

    // ----- 저장 + (있으면) 즉시 장착 반영 -----
    private void SaveLoadoutAndApply()
    {
        var d = GetCurrent();
        if (d == null) return;

        if (slot == SlotPhase.Stealth) LoadoutProfile.Instance?.SetStealth(d.weaponName, save: true);
        else LoadoutProfile.Instance?.SetCombat(d.weaponName, save: true);

        var wm = WeaponManager.Instance;
        if (wm)
        {
            wm.ApplyLoadoutFromProfile();
            var gm = GameManager.Instance;
            var phase = gm ? gm.CurrentPhase
                           : (slot == SlotPhase.Combat ? GamePhase.Combat : GamePhase.Stealth);
            wm.ApplyPhaseWeapon(phase);
        }
    }

    private GunData GetCurrent() => (candidates.Count == 0) ? null : candidates[currentItemIndex];
}
