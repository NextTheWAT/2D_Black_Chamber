using UnityEngine;

[System.Serializable]
public class StageReward //���������� Ŭ���� ���� ������
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
