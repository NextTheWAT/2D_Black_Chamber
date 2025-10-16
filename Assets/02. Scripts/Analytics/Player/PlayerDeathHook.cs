using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

public class PlayerDeathHook : MonoBehaviour
{
    [Header("Analytics")]
    [SerializeField] string stageIdOverride = ""; // 비워두면 씬 이름 자동 사용
    [SerializeField] string gameplayMode = ""; // 외부에서 SetGameplayMode로 갱신

    string stageId; // 실제 전송용

    void Awake()
    {
        stageId = string.IsNullOrWhiteSpace(stageIdOverride)
            ? SceneManager.GetActiveScene().name
            : stageIdOverride;
    }

    void OnEnable() { var gm = GameManager.Instance; if (gm != null) gm.OnPhaseChanged += PhaseChanged; }
    void OnDisable() { var gm = GameManager.Instance; if (gm != null) gm.OnPhaseChanged -= PhaseChanged; }
    void PhaseChanged(Constants.GamePhase p) => SetGameplayMode(p == Constants.GamePhase.Combat);

    public void SetGameplayMode(bool combat) => gameplayMode = combat ? "combat" : "stealth";

    // 사망 처리 시점에서 이 메서드만 호출하면 끝!
    public void OnDie(string killerId)
    {
        // 1) 실패(리트라이) 먼저
        var tracker = FindAnyObjectByType<StageRunTracker>();
        if (tracker != null) tracker.ReportFailed();
        else Debug.Log("[AN] No StageRunTracker in scene. Skip stage_failed.");

        // 2) 안전 가드
        if (string.IsNullOrEmpty(killerId)) killerId = "unknown";
        if (string.IsNullOrEmpty(stageId))
            stageId = SceneManager.GetActiveScene().name;

        // 3) player_death 전송
        var pos = (Vector2)transform.position;
        GA.PlayerDeath(stageId, pos, killerId, gameplayMode);

#if UNITY_WEBGL || UNITY_EDITOR
        Unity.Services.Analytics.AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"[AN] SENT player_death sid={stageId}, pos={pos}, killer={killerId}, mode={gameplayMode}");
    }
}
