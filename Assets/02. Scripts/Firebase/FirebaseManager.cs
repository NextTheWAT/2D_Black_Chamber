using Proyecto26;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class User
{
    public string userName;
    public int score;
    public float playTime;

    public User(string userName, int score, float playTime)
    {
        this.userName = userName;
        this.score = score;
        this.playTime = playTime;
    }
}

public class FirebaseManager : Singleton<FirebaseManager>
{
    public Action EnemyDataLoaded;

    public Dictionary<int, EnemyData> enemyDataDict = new();
    private const string userDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/users";
    private const string enemyDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/EnemyData.json";
    public string UserID => SystemInfo.deviceUniqueIdentifier;

    private bool enemyDataLoaded = false;
    private bool userDataLoaded = false;
    public bool IsInitialized => enemyDataLoaded;

    protected override void Initialize()
    {
        base.Initialize();
        UpdateEnemyDatas();
        User user = new("Player", 0, 0f);
        PutUser(user, UserID);
    }

    public EnemyData GetEnemyData(int id) => enemyDataDict[id];

    public void UpdateEnemyDatas()
    {
        RestClient.Get(enemyDataURL).Then(response =>
        {
            var responseJson = response.Text;
            enemyDataDict = JsonConvert.DeserializeObject<Dictionary<int, EnemyData>>(responseJson);
            enemyDataLoaded = true;
            EnemyDataLoaded?.Invoke();

            foreach (var kvp in enemyDataDict)
                ConditionalLogger.Log($"ID: {kvp.Key}, Name: {kvp.Value.enemyName}");
        });
    }

    public void GetUser(string userId, Action<User> callback)
        => RestClient.Get<User>($"{userDataURL}/{userId}.json").Then(response => callback?.Invoke(response));

    public void PostUser(User user, string userId, Action callback = null)
        => RestClient.Post<User>($"{userDataURL}/{userId}.json", user).Then(response => callback?.Invoke());


    public void PutUser(User user, string userId, Action callback = null)
        => RestClient.Put<User>($"{userDataURL}/{userId}.json", user).Then(response => callback?.Invoke());
}
