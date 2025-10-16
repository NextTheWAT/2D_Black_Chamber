// ModeSwitchTracker.cs
using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;
using Constants;

public class ModeSwitchTracker : MonoBehaviour
{
    [SerializeField] Transform player;
    string stageId;
    bool _isStealth = true;
    float _stealthStart;

    void Awake()
    {
        stageId = SceneManager.GetActiveScene().name;           // stage_id 자동
        if (player == null) player = FindAnyObjectByType<GameManager>()?.Player; // 자동 할당
    }
    void OnEnable()
    {
        _isStealth = true; _stealthStart = Time.time;
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += HandlePhaseChanged; // 구독
    }
    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= HandlePhaseChanged; // 해제
    }

    void HandlePhaseChanged(GamePhase phase)
    {
        if (phase == GamePhase.Combat) OnPhaseChangedToCombat();
        else OnPhaseChangedToStealth();
    }

    public void OnPhaseChangedToCombat()
    {
        if (!_isStealth) return;
        float timeInStealth = Time.time - _stealthStart;
        GA.ModeSwitch(stageId, (Vector2)player.position, timeInStealth);
#if UNITY_WEBGL
        AnalyticsService.Instance.Flush();
#endif
        _isStealth = false;
    }
    public void OnPhaseChangedToStealth()
    {
        _isStealth = true;
        _stealthStart = Time.time;
    }
}
