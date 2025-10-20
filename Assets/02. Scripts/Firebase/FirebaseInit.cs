
#if UNITY_EDITOR
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{

    public string userName;
    public int userScore;

    DatabaseReference dbReference;

    void Start()
    {
        // Firebase 초기화
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                // 데이터 저장 예시
                // SaveData(userName, userScore);

                // 데이터 불러오기
                LoadData();
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + dependencyStatus);
            }
        });
    }

    void LoadData()
    {
        Debug.Log("데이터 불러오기 시작");
        dbReference.Child("EnemyData").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터 불러오기 실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("전체 데이터: " + snapshot.GetRawJsonValue());
                Debug.Log($"Key: {snapshot.Key} Value: {snapshot.Value} Children: {snapshot.Children} ChildrenCount {snapshot.ChildrenCount}");
                
                foreach (var item in snapshot.Children)
                {
                    EnemySheetData data = ScriptableObject.CreateInstance<EnemySheetData>();
                    data.name = item.Key;
                    JsonUtility.FromJsonOverwrite(item.GetRawJsonValue(), data);
                    ScriptableObjectUtility.SaveScriptableObject(data, $"Assets/Data/Enemy/{item.Key}.asset");
                }
            }
        });
    }

}
#endif