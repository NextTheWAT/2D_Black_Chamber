using System.Collections.Generic;
using UnityEngine;

public class StageRewardManager : Singleton<StageRewardManager>
    //���������� Ŭ���� ���� ����
{

    private Dictionary<int, StageReward> rewards = new();

    private void Awake()
    {
        InitRewards();
    }

    private void InitRewards()
    {
        rewards[1] = new StageReward(1000, 700, 500);
        rewards[2] = new StageReward(2000, 1400, 1000);
        rewards[3] = new StageReward(3000, 2100, 1500);
    }

    public StageReward GetReward(int stageNumber)
    {
        if (rewards.TryGetValue(stageNumber, out StageReward reward))
            return reward;

        Debug.LogWarning($"[StageRewardManager] �������� {stageNumber} ���� ������ ����");
        return null;
    }
}
