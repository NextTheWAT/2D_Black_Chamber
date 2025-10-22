using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string userName;
    public int money;
    public float playTime;
    public Dictionary<int, ClearResultData> clearDatas;

    public UserData(string userName, int money, float playTime)
    {
        this.userName = userName;
        this.money = money;
        this.playTime = playTime;
        clearDatas = new();
    }
}
