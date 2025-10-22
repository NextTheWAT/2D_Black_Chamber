using UnityEngine;

public class ClearResultData
{
    public int stageNumber;
    public int killCount;
    public string clearStateText;
    public float elapsedSeconds;
    public int rewardDollar;
    
    public ClearResultData(int stage, int kill, string stateText, float seconds, int reward)
    {
        stageNumber = stage;
        killCount = kill;
        clearStateText = stateText;
        elapsedSeconds = seconds;
        rewardDollar = reward;
    }
}
