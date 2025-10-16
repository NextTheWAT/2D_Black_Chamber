using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

public class PlayerDeathHook : MonoBehaviour
{
    [Header("Analytics")]
    [SerializeField] string stageIdOverride = ""; // ����θ� �� �̸� �ڵ� ���
    [SerializeField] string gameplayMode = "stealth"; // �ܺο��� SetGameplayMode�� ����

    string stageId; // ���� ���ۿ�

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

    // ��� ó�� �������� �� �޼��常 ȣ���ϸ� ��!
    public void OnDie(string killerId)
    {
        FindAnyObjectByType<StageRunTracker>()?.ReportFailed();

        GA.PlayerDeath(stageId, (Vector2)transform.position, killerId, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
    }
}
