// GA.cs
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;

public static class GA
{
    static void AddCommon(Dictionary<string, object> p)
    {

    }

    static Vector2 Snap(Vector2 v, float step = 0.5f)
    {
        float sx = Mathf.Round(v.x / step) * step;
        float sy = Mathf.Round(v.y / step) * step;
        return new Vector2(sx, sy);
    }

    // 공통 전송 (v6: RecordEvent)
    static void Send(string eventName, Dictionary<string, object> p)
    {
        var ev = new CustomEvent(eventName);
        foreach (var kv in p) ev.Add(kv.Key, kv.Value);
        AnalyticsService.Instance.RecordEvent(ev);

#if UNITY_EDITOR
        Debug.Log($"[AN] GA.Send {eventName} -> " +
                  string.Join(", ", p.Select(kv => $"{kv.Key}:{kv.Value}")));
#endif
    }

    // 1) player_death
    public static void PlayerDeath(string stage_id, Vector2 position, string killer_id, string gameplay_mode)
    {
        var sp = Snap(position, 0.5f);
        var p = new Dictionary<string, object>
        {
            { "stage_id", stage_id },
            { "position_x", sp.x },
            { "position_y", sp.y },
            { "killer_id", killer_id },
            { "gameplay_mode", gameplay_mode } // "stealth" | "combat"
        };
        AddCommon(p);
        Send("player_death", p);
    }

    // 2) stage_complete
    public static void StageComplete(string stage_id, int attempt_count, float play_time, int kill_enemies_count, string gameplay_mode)
    {
        var p = new Dictionary<string, object>
        {
            { "stage_id", stage_id },
            { "attempt_count", attempt_count },
            { "play_time", Mathf.RoundToInt(play_time) },
            { "kill_enemies_count", kill_enemies_count },
            { "gameplay_mode", gameplay_mode }
        };
        AddCommon(p);
        Send("stage_complete", p);
    }

    // 3) stage_failed
    public static void StageFailed(string stage_id, int attempt_count, float play_time, int kill_enemies_count, string gameplay_mode)
    {
        var p = new Dictionary<string, object>
        {
            { "stage_id", stage_id },
            { "attempt_count", attempt_count },
            { "play_time", Mathf.RoundToInt(play_time) },
            { "kill_enemies_count", kill_enemies_count },
            { "gameplay_mode", gameplay_mode }
        };
        AddCommon(p);
        Send("stage_failed", p);
    }

    // 4) mode_switch
    public static void ModeSwitch(string stage_id, Vector2 switch_position, float time_in_stealth)
    {
        var sp = Snap(switch_position, 0.5f);
        var p = new Dictionary<string, object>
        {
            { "stage_id", stage_id },
            { "switch_position_x", sp.x },
            { "switch_position_y", sp.y },
            { "time_in_stealth", Mathf.RoundToInt(time_in_stealth) }
        };
        AddCommon(p);
        Send("mode_switch", p);
    }
}
