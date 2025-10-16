using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;
using Constants;


public class StageRunTracker : MonoBehaviour
{
    [Header("IDs")]
    [SerializeField] string stageId = "stage_1";
    [SerializeField] string gameplayMode = "stealth"; // 외부에서 갱신
    public void SetGameplayMode(bool combat) => gameplayMode = combat ? "combat" : "stealth";

    float _startTime;
    int _attemptCount;
    int _killCount;
    bool _ended;

    void Awake()
    {
        stageId = SceneManager.GetActiveScene().name;   // stage_id 자동
    }

    void OnEnable()
    {
        _attemptCount = PlayerPrefs.GetInt($"attempt_{stageId}", 0) + 1;
        PlayerPrefs.SetInt($"attempt_{stageId}", _attemptCount);
        _startTime = Time.time;
        _killCount = 0;
        _ended = false;

        var gm = GameManager.Instance;                   // 모드 변경 구독
        if (gm != null)
            gm.OnPhaseChanged += phase => SetGameplayMode(phase == GamePhase.Combat);
    }
    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null)
            gm.OnPhaseChanged -= phase => SetGameplayMode(phase == GamePhase.Combat);
    }

    // 적 처치 시 적 스크립트에서 호출: tracker.AddKill();
    public void AddKill() => _killCount++;

    // 클리어 순간(연출 직전/직후) 호출
    public void ReportComplete()
    {
        if (_ended) return;
        _ended = true;
        var playSec = Time.time - _startTime;
        GA.StageComplete(stageId, _attemptCount, playSec, _killCount, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
        PlayerPrefs.DeleteKey($"attempt_{stageId}"); // 다음 스테이지 넘어간다면 초기화
    }

    // 실패 확정 순간 호출(게임오버, 시간초과 등)
    public void ReportFailed()
    {
        if (_ended) return;
        _ended = true;
        var playSec = Time.time - _startTime;
        GA.StageFailed(stageId, _attemptCount, playSec, _killCount, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
        // 실패면 attempt 카운트 유지(다음 리트라이에서 +1)
    }
}
