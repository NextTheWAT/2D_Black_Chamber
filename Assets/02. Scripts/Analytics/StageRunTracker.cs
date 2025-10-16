using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;
using Constants;


public class StageRunTracker : MonoBehaviour
{
    [Header("IDs")]
    [SerializeField] string stageId = "stage_1";
    [SerializeField] string gameplayMode = "stealth"; // �ܺο��� ����
    public void SetGameplayMode(bool combat) => gameplayMode = combat ? "combat" : "stealth";

    float _startTime;
    int _attemptCount;
    int _killCount;
    bool _ended;

    void Awake()
    {
        stageId = SceneManager.GetActiveScene().name;   // stage_id �ڵ�
    }

    void OnEnable()
    {
        _attemptCount = PlayerPrefs.GetInt($"attempt_{stageId}", 0) + 1;
        PlayerPrefs.SetInt($"attempt_{stageId}", _attemptCount);
        _startTime = Time.time;
        _killCount = 0;
        _ended = false;

        var gm = GameManager.Instance;                   // ��� ���� ����
        if (gm != null)
            gm.OnPhaseChanged += phase => SetGameplayMode(phase == GamePhase.Combat);
    }
    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null)
            gm.OnPhaseChanged -= phase => SetGameplayMode(phase == GamePhase.Combat);
    }

    // �� óġ �� �� ��ũ��Ʈ���� ȣ��: tracker.AddKill();
    public void AddKill() => _killCount++;

    // Ŭ���� ����(���� ����/����) ȣ��
    public void ReportComplete()
    {
        if (_ended) return;
        _ended = true;
        var playSec = Time.time - _startTime;
        GA.StageComplete(stageId, _attemptCount, playSec, _killCount, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
        PlayerPrefs.DeleteKey($"attempt_{stageId}"); // ���� �������� �Ѿ�ٸ� �ʱ�ȭ
    }

    // ���� Ȯ�� ���� ȣ��(���ӿ���, �ð��ʰ� ��)
    public void ReportFailed()
    {
        if (_ended) return;
        _ended = true;
        var playSec = Time.time - _startTime;
        GA.StageFailed(stageId, _attemptCount, playSec, _killCount, gameplayMode);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
        // ���и� attempt ī��Ʈ ����(���� ��Ʈ���̿��� +1)
    }
}
