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
        stageId = SceneManager.GetActiveScene().name;
        if (!StageRunTracker.IsGameplayScene(stageId))   // ¿ß¿« «Ô∆€∏¶ public static¿∏∑Œ ªÃæ∆µµ µ 
        {
            enabled = false;
            return;
        }
        if (player == null) player = GetComponent<Transform>();
    }
    void OnEnable()
    {
        _isStealth = true; _stealthStart = Time.time;
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += HandlePhaseChanged; // ±∏µ∂
    }
    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= HandlePhaseChanged; // «ÿ¡¶
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
