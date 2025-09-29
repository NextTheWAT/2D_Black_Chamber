using UnityEngine;

public class GameStats : Singleton<GameStats>
{
    private float startTime;
    private int killCount;

    public void StartStage()
    {
        startTime = Time.time;
        killCount = 0;
#if UNITY_EDITOR
        Debug.Log($"[GameStats] StartStage at {startTime:F2}");
#endif
    }

    public void AddKill()
    {
        killCount++;
#if UNITY_EDITOR
        Debug.Log($"[GameStats] Kill++ => {killCount}");
#endif
    }

    public ClearResultData BuildClearResult(string clearStateText, int rewardDollar)
    {
        float elapsed = Mathf.Max(0f, Time.time - startTime);
        return new ClearResultData(killCount, clearStateText, elapsed, rewardDollar);
    }
}
