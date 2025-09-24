using UnityEngine;

public class ClearResultData
{
    public int killCount;
    public string clearStateText;
    public float elapsedSeconds;
    public int rewardDollar;
    
    public ClearResultData(int kill, string stateText, float seconds, int reward)
    {
        killCount = kill;
        clearStateText = stateText;
        elapsedSeconds = seconds;
        rewardDollar = reward;
    }
}
