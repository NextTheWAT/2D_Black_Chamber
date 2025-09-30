using UnityEngine;

[System.Serializable]
public class StageReward //스테이지별 클리어 보상 데이터
{
    public int stealthReward;
    public int combatReward;
    public int stageMoney;

    public StageReward(int stealth, int combat, int money)
    {
        stealthReward = stealth;
        combatReward = combat;
        stageMoney = money;
    }
}
