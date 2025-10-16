using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

public class PlayerDeathHook : MonoBehaviour
{
    [Header("Analytics")]
    [SerializeField] string stageIdOverride = ""; // 비워두면 씬 이름 자동 사용
    [SerializeField] string gameplayMode = "stealth"; // 외부에서 SetGameplayMode로 갱신

    string stageId; // 실제 전송용

    void Awake()
    {
        stageId = string.IsNullOrWhiteSpace(stageIdOverride)
            ? SceneManager.GetActiveScene().name
            : stageIdOverride;
    }

    void OnEnable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += PhaseChanged;
    }
    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= PhaseChanged;
    }
    void PhaseChanged(Constants.GamePhase phase) =>
        SetGameplayMode(phase == Constants.GamePhase.Combat);

    public void SetGameplayMode(bool combat) => gameplayMode = combat ? "combat" : "stealth";

    // 사망 처리 시점에서 이 메서드만 호출하면 끝!
    public void OnDie(string killerId)
    {
        FindAnyObjectByType<StageRunTracker>()?.ReportFailed();

        GA.PlayerDeath(stageId, (Vector2)transform.position, killerId, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
    }
}
