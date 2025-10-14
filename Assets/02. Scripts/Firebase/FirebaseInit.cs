using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{

    public string userName;
    public int userScore;

    DatabaseReference dbReference;

    void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                // 데이터 저장 예시
                SaveData(userName, userScore);

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
        Debug.Log("dbReference: " + dbReference.Child("users"));
        dbReference.Root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터 불러오기 실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("전체 데이터: " + snapshot.GetRawJsonValue());
            }
        });

        // 예시: /users/user1/name 값을 불러오기
        dbReference.Child("users").Child("user1").Child("name").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터 불러오기 실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("불러온 데이터: " + snapshot.Value);
            }
        });
    }

    void SaveData(string userName, int userScore)
    {
        dbReference.Child("users").Child("user1").Child("name").SetValueAsync(userName);
        dbReference.Child("users").Child("user1").Child("score").SetValueAsync(userScore);
    }

}
