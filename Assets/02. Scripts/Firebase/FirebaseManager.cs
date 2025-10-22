using Proyecto26;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LeaderBoard
{


}


public class FirebaseManager : Singleton<FirebaseManager>
{
    public Action EnemyDataLoaded;
    public Action UserDataLoaded;

    public Dictionary<int, EnemyData> enemyDataDict = new();
    public Dictionary<string, UserData> userDataDict = new();
    private const string userDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/users";
    private const string enemyDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/EnemyData.json";
    private const string leaderBoardURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/LeaderBoard";
    public string UserID => SystemInfo.deviceUniqueIdentifier;
    private UserData userData;

    private bool isEnemyDataLoaded = false;
    private bool isUserDataLoaded = false;

    public bool IsEnemyDataLoaded => isEnemyDataLoaded;
    public bool IsUserDataLoaded => isUserDataLoaded;

    public bool IsInitialized => isEnemyDataLoaded && isUserDataLoaded;

    protected override void Initialize()
    {
        base.Initialize();
        UpdateEnemyDatas();
        UpdateUserDatas();
        userData = new("Player", 0, 0f);
        PutUser(userData, UserID);
    }

    public EnemyData GetEnemyData(int id) => enemyDataDict[id];

    public void UpdateEnemyDatas()
    {
        RestClient.Get(enemyDataURL).Then(response =>
        {
            enemyDataDict = JsonConvert.DeserializeObject<Dictionary<int, EnemyData>>(response.Text);
            isEnemyDataLoaded = true;
            EnemyDataLoaded?.Invoke();

            foreach (var kvp in enemyDataDict)
                ConditionalLogger.Log($"ID: {kvp.Key}, Name: {kvp.Value.enemyName}");
        });
    }

    public void UpdateUserDatas()
    {
        RestClient.Get(userDataURL + ".json").Then(response =>
        {
            userDataDict = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(response.Text);
            isUserDataLoaded = true;
            UserDataLoaded?.Invoke();

            foreach (var kvp in userDataDict)
                ConditionalLogger.Log($"UserID: {kvp.Key}, UserName: {kvp.Value.userName}, Score: {kvp.Value.money}, PlayTime: {kvp.Value.playTime}");
        });
    }

    public void UploadClearData(ClearResultData data)
    {
        userData.clearDatas[data.stageNumber] = data;
        PutUser(userData, UserID);
    }

    public void GetUser(string userId, Action<UserData> callback)
        => RestClient.Get<UserData>($"{userDataURL}/{userId}.json").Then(response => callback?.Invoke(response));

    public void PostUser(UserData user, string userId, Action callback = null)
        => RestClient.Post<UserData>($"{userDataURL}/{userId}.json", user).Then(response => callback?.Invoke());

    public void PutUser(UserData user, string userId, Action callback = null)
        => RestClient.Put<UserData>($"{userDataURL}/{userId}.json", user).Then(response => callback?.Invoke());
}
