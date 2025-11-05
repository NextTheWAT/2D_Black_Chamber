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

    public Color[] backgroundGradeColors; // 등급별 배경 색상 배열
    public Color[] borderGradeColors;     // 등급별 테두리 색상 배열

    [Space(20)]
    public TMP_Text weaponNameText;
    public Image backImage;

    [Space(20)]
    public TMP_Text categoryText;
    public TMP_Text damageText;
    public TMP_Text rpmText;
    public TMP_Text noiseText;
    public TMP_Text reloadSpeedText;
    public TMP_Text ammoCapacityText;

    [Space(20)]
    public TMP_Text accuracyText;
    public TMP_Text precisionText;
    public TMP_Text stabilityText;

    [Space(20)]
    public TMP_Text aimAccuracyText;
    public TMP_Text aimPrecisionText;
    public TMP_Text aimStabilityText;
    public TMP_Text aimDistanceText;

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
            // 빈칸 처리
            if (weaponNameText) weaponNameText.text = "-";
            if (categoryText) categoryText.text = "-";
            if (damageText) damageText.text = "-";
            if (rpmText) rpmText.text = "-";
            if (noiseText) noiseText.text = "-";
            if (reloadSpeedText) reloadSpeedText.text = "-";
            if (ammoCapacityText) ammoCapacityText.text = "-";
            if (accuracyText) accuracyText.text = "-";
            if (precisionText) precisionText.text = "-";
            if (stabilityText) stabilityText.text = "-";
            if (aimAccuracyText) aimAccuracyText.text = "-";
            if (aimPrecisionText) aimPrecisionText.text = "-";
            if (aimStabilityText) aimStabilityText.text = "-";
            if (aimDistanceText) aimDistanceText.text = "-";

            return;
        }

        // 표시 이름: weaponName → displayName(구스펙) → ScriptableObject name
        weaponNameText.text = d.weaponName.ToString();

        // 분류: phaseTag / subType
        // var cat = ReadStr(gun, "phaseTag") ?? "Unknown";
        var cat = d.prefabInfo.phaseTag;
        var sub = ReadStr(d, "subType");
        categoryText.text = string.IsNullOrEmpty(sub) ? $"분류 : {cat}" : $"분류 : {cat} / {sub}";

        damageText.text = $"데미지 : {ReadStr(d, "damage") ?? "-"}";
        rpmText.text = $"RPM : {ReadStr(d, "rpm") ?? ReadStr(d, "fireRate") ?? "-"}";
        // 소음: volume(신) → noise(구)
        noiseText.text = $"소음 : {ReadStr(d, "volume") ?? ReadStr(d, "noise") ?? "-"}";
        // 재장전속도: reloadSpeed(신, %) → reloadSpeedMul(구)
        reloadSpeedText.text = $"재장전 시간 : {ReadStr(d, "reloadSpeed") ?? ReadStr(d, "reloadSpeedMul") ?? "-"}";
        // 장탄수: maxAmmo(신) → maxMagazine(구) → magazine(아주 구)
        ammoCapacityText.text = $"장탄수 : {ReadStr(d, "maxAmmo") ?? ReadStr(d, "maxMagazine") ?? ReadStr(d, "magazine") ?? "-"}";


        // 정확도: accuracy(신) → aimAccuracy → accuracyDeg(구)
        accuracyText.text = $"정확도 : {ReadStr(d, "accuracy") ?? "-"}";
        // 정밀도: precision(신, 0~100) → spread(구) → accuracyDeg(구)
        precisionText.text = $"정밀도 : {ReadStr(d, "precision") ?? "-"}";
        // 안정성 : stability
        stabilityText.text = $"안정성 : {ReadStr(d, "stability") ?? "-"}";


        // 조준 시 정확도: aimAccuracy(신) → aimAccuracy → accuracyDeg(구)
        aimAccuracyText.text = $"조준 시 정확도 : {ReadStr(d, "aimAccuracy") ?? "-"}";
        // 조준 시 정밀도 : aimPrecision(신, 0~100)
        aimPrecisionText.text = $"조준 시 정밀도 : {ReadStr(d, "aimPrecision") ?? "-"}";
        // 조준 시 안정성 : aimStability
        aimStabilityText.text = $"조준 시 안정성 : {ReadStr(d, "aimStability") ?? "-"}";

        // 조준 배율 : aimDistance
        string aimDistanceStr = ReadStr(d, "aimDistance") ?? "-";
        float aimDistanceValue = float.Parse(aimDistanceStr) * 100f;

        aimDistanceText.text = $"조준 배율 : {aimDistanceValue}%";

        // 등급별 색상 적용
        int gradeIndex = Mathf.Clamp(d.Grade, 0, backgroundGradeColors.Length - 1);
        backImage.color = borderGradeColors[gradeIndex];
        weaponNameText.color = backgroundGradeColors[gradeIndex];

    }
    // --- helpers ---

    public static string ReadStr(object obj, string field)
    {
        var f = obj.GetType().GetField(field);
        if (f == null) return null;
        var v = f.GetValue(obj);
        return v != null ? v.ToString() : null;
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
