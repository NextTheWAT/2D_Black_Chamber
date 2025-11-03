using UnityEngine;

public class GameStats : Singleton<GameStats>
{
    [SerializeField] private float startTime;
    [SerializeField] private int killCount;
    [SerializeField] private int moneyCollected;

    public int KillCount => killCount;

    public void AddMoney(int amount)
    {
        moneyCollected += amount;
    }

    public void StartStage()
    {
        startTime = Time.time;
        killCount = 0;
        moneyCollected = 0;
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

    public ClearResultData BuildClearResult(int stageNumber, string clearStateText, int rewardDollar)
    {
        float elapsed = Mathf.Max(0f, Time.time - startTime);
        int totalReward = rewardDollar + moneyCollected;
        return new ClearResultData(stageNumber, killCount, clearStateText, elapsed, totalReward);
    }
}
