using Constants;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : Singleton<MissionManager>
{
    [Header("Runtime")]
    [SerializeField] private int remainingTargets = 0;
    [SerializeField] private int remainingEnemies = 0;
    [SerializeField] private MissionPhase phase = MissionPhase.Assassination;

    [Header("Mission Bell")]
    [SerializeField] private GameObject missionBell_Obj;
    [SerializeField] private GameObject missionBell_Obj2;

    [Header("Combat Alert UI")]
    [SerializeField] private GameObject combatAlertUI;   // 난전 진입 시 3초 표시할 UI
    [SerializeField] private float combatAlertDuration = 3f;
    private Coroutine _combatUiRoutine;

    [SerializeField] private UIKey UIKey;

    public int RemainingTargets => remainingTargets;
    public int RemainingEnemies => remainingEnemies;

    public MissionPhase Phase
    {
        get => phase;
        set => SetPhase(value);
    }

    public event Action<int> OnTargetsChanged;
    public event Action<int> OnEnemiesChanged;
    public event Action<MissionPhase> OnPhaseChanged;


    private void Start()
    {
        if (missionBell_Obj) missionBell_Obj.SetActive(false);
        if (missionBell_Obj2) missionBell_Obj2.SetActive(false);

        // 기본은 꺼둡니다.
        if (combatAlertUI) combatAlertUI.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded_UpdateUIKey;

        // 난전 상태 변경 이벤트 구독
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += HandleGamePhaseChanged;   // Combat/Stealth 전환 수신  :contentReference[oaicite:2]{index=2}
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_UpdateUIKey;

        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= HandleGamePhaseChanged;

        if (_combatUiRoutine != null) StopCoroutine(_combatUiRoutine);
    }
    private void HandleGamePhaseChanged(GamePhase phase)
    {
        // 난전 진입 시 3초간 표시, 스텔스 복귀 시 즉시 숨김
        if (phase == GamePhase.Combat)
        {
            if (_combatUiRoutine != null) StopCoroutine(_combatUiRoutine);
            _combatUiRoutine = StartCoroutine(ShowCombatAlertUI());
        }
        else
        {
            if (_combatUiRoutine != null) StopCoroutine(_combatUiRoutine);
            _combatUiRoutine = null;
            if (combatAlertUI) combatAlertUI.SetActive(false);
        }
    }

    private IEnumerator ShowCombatAlertUI()
    {
        if (UIKey == UIKey.Game && combatAlertUI)  // 게임 UI일 때만 노출  :contentReference[oaicite:3]{index=3}
        {
            combatAlertUI.SetActive(true);
            yield return new WaitForSeconds(combatAlertDuration);
            combatAlertUI.SetActive(false);
        }
        _combatUiRoutine = null;
    }

    // --- 암살 대상 ---
    public void TargetActivated()
    {
        remainingTargets++;
        OnTargetsChanged?.Invoke(remainingTargets);
    }

    public void TargetDeactivated()
    {
        if (remainingTargets > 0) remainingTargets--;
        OnTargetsChanged?.Invoke(remainingTargets);
        EvaluatePhase();
    }

    // --- 적 ---
    public void EnemyActivated()
    {
        remainingEnemies++;
        OnEnemiesChanged?.Invoke(remainingEnemies);
    }

    public void EnemyDeactivated()
    {
        if (remainingEnemies > 0) remainingEnemies--;
        OnEnemiesChanged?.Invoke(remainingEnemies);

    }

    // 암살 대상이 0이면 도주 단계로
    private void EvaluatePhase()
    {
        if (phase == MissionPhase.Assassination && remainingTargets <= 0)
            SetPhase(MissionPhase.Escape);
    }

    public void SetPhase(MissionPhase next)
    {
        if (phase == next) return;
        phase = next;
        OnPhaseChanged?.Invoke(phase);
#if UNITY_EDITOR
        Debug.Log($"[MissionManager] Phase -> {phase}");
#endif

        if (phase == MissionPhase.Escape)
        {
            StartCoroutine(Mission_Clear_Text());
        }

    }

    // 선택: 초기화가 필요할 때
    public void ResetCounts(int targets = 0, int enemies = 0, MissionPhase startPhase = MissionPhase.Assassination)
    {
        remainingTargets = Mathf.Max(0, targets);
        remainingEnemies = Mathf.Max(0, enemies);
        SetPhase(startPhase);
        OnTargetsChanged?.Invoke(remainingTargets);
        OnEnemiesChanged?.Invoke(remainingEnemies);
    }

    private void OnSceneLoaded_UpdateUIKey(Scene scene, LoadSceneMode mode)
    {
        var init = FindFirstObjectByType<SceneInitializer>(FindObjectsInactive.Include);
        if (init != null)
            UIKey = init.ActiveUI;   // 씬의 SceneInitializer가 정한 UIKey로 갱신
    }


    //예비 미션 클리어 텍스트 처리

    IEnumerator Mission_Clear_Text()
    {
        yield return new WaitForSeconds(0.5f);

        if(UIKey == UIKey.Game)
        {
            missionBell_Obj.SetActive(true);
            yield return new WaitForSeconds(2f);
            missionBell_Obj.SetActive(false);
            missionBell_Obj2.SetActive(true);
            yield return new WaitForSeconds(2f);
            missionBell_Obj2.SetActive(false);
        }
    }
}
