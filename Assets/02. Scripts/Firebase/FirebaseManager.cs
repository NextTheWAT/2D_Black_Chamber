using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Proyecto26;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public Action EnemyDataLoaded;
    public Action GunDataLoaded;
    public Action UserDataLoaded;

    public Dictionary<int, EnemyData> enemyDataDict = new();
    public Dictionary<int, GunData> gunDataDict = new();
    public Dictionary<int, EnemyData> attachmentDataDict = new();
    public Dictionary<string, UserData> userDataDict = new();

    private const string enemyDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/EnemyData.json";
    private const string gunDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/GunData.json";
    private const string userDataURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/UserData";
    private const string leaderBoardURL = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/LeaderBoard";
    public string UserID => SystemInfo.deviceUniqueIdentifier;
    private UserData myUserData;

    private bool isEnemyDataLoaded = false;
    private bool isGunDataLoaded = false;
    private bool isUserDataLoaded = false;

    public bool IsEnemyDataLoaded => isEnemyDataLoaded;
    public bool IsGunDataLoaded => isGunDataLoaded;
    public bool IsUserDataLoaded => isUserDataLoaded;
    public bool IsInitialized => isEnemyDataLoaded && isUserDataLoaded;

    protected override void Initialize()
    {
        base.Initialize();
        UpdateGunDatas();
        UpdateEnemyDatas();
        UpdateUserDatas();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        if (!IsInitialized) return;
        Debug.Log("Scene Loaded - Save My User Data");
        SaveMyUserData();
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

    public void UpdateGunDatas()
    {
        RestClient.Get(gunDataURL).Then(response =>
        {
            gunDataDict = JsonConvert.DeserializeObject<Dictionary<int, GunData>>(response.Text);
            isGunDataLoaded = true;
            GunDataLoaded?.Invoke();

            foreach (var kvp in gunDataDict)
                ConditionalLogger.Log($"ID: {kvp.Key}, Name: {kvp.Value.weaponName}");
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
                ConditionalLogger.Log($"UserID: {kvp.Key}, UserName: {kvp.Value.userName}, Score: {kvp.Value.money}, PlayTime: {kvp.Value.totalPlayTime}");
        }).Finally(() =>
        {
            if (userDataDict.ContainsKey(UserID))
            {
                myUserData = userDataDict[UserID];
            }
            else
            {
                myUserData = new UserData("Player", 0, 0f);
            }
        });
    }

    public void SaveMyUserData()
    {
        myUserData.money = MoneyManager.Instance.Balance;
        string json = JsonConvert.SerializeObject(myUserData, Formatting.Indented);
        RestClient.Put($"{userDataURL}/{UserID}.json", json);
    }

    public void UploadLeaderBoard(ClearResultData data)
    {
        string json = JsonConvert.SerializeObject(myUserData, Formatting.Indented);
        RestClient.Put($"{leaderBoardURL}/{data.stageNumber}.json", json);
    }

    public void UploadClearData(ClearResultData data)
    {
        // 클리어 시간 기록이 기존 기록보다 느리면 업로드하지 않음
        if (myUserData.clearDatas.ContainsKey(data.stageNumber))
        {
            var existingData = myUserData.clearDatas[data.stageNumber];
            if (data.elapsedSeconds > existingData.elapsedSeconds) return;
        }

        myUserData.clearDatas[data.stageNumber] = data;
        SaveMyUserData();
    }



}
