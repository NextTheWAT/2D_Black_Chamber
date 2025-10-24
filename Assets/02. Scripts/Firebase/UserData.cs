using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public string userName;
    public int money;
    public float totalPlayTime;
    public Dictionary<int, ClearResultData> clearDatas;

    public UserData(string userName, int money, float totalPlayTime)
    {
        this.userName = userName;
        this.money = money;
        this.totalPlayTime = totalPlayTime;
        clearDatas = new();
    }
}
