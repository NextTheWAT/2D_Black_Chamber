using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

public class PlayerDeathHook : MonoBehaviour
{
    [Header("Analytics")]
    [SerializeField] string stageIdOverride = ""; // ����θ� �� �̸� �ڵ� ���
    [SerializeField] string gameplayMode = ""; // �ܺο��� SetGameplayMode�� ����

    string stageId; // ���� ���ۿ�

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

    // ��� ó�� �������� �� �޼��常 ȣ���ϸ� ��!
    public void OnDie(string killerId)
    {
        // 1) ����(��Ʈ����) ����
        var tracker = FindAnyObjectByType<StageRunTracker>();
        if (tracker != null) tracker.ReportFailed();
        else Debug.Log("[AN] No StageRunTracker in scene. Skip stage_failed.");

        // 2) ���� ����
        if (string.IsNullOrEmpty(killerId)) killerId = "unknown";
        if (string.IsNullOrEmpty(stageId))
            stageId = SceneManager.GetActiveScene().name;

        // 3) player_death ����
        var pos = (Vector2)transform.position;
        GA.PlayerDeath(stageId, pos, killerId, gameplayMode);

#if UNITY_WEBGL || UNITY_EDITOR
        Unity.Services.Analytics.AnalyticsService.Instance.Flush();
#endif

        Debug.Log($"[AN] SENT player_death sid={stageId}, pos={pos}, killer={killerId}, mode={gameplayMode}");
    }
}
