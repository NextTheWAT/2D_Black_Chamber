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
        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                // ������ ���� ����
                SaveData(userName, userScore);

                // ������ �ҷ�����
                LoadData();
            }
            else
            {
                Debug.LogError("Firebase �ʱ�ȭ ����: " + dependencyStatus);
            }
        });
    }

    void LoadData()
    {
        Debug.Log("������ �ҷ����� ����");
        Debug.Log("dbReference: " + dbReference.Child("users"));
        dbReference.Root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("������ �ҷ����� ����");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("��ü ������: " + snapshot.GetRawJsonValue());
            }
        });

        // ����: /users/user1/name ���� �ҷ�����
        dbReference.Child("users").Child("user1").Child("name").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("������ �ҷ����� ����");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("�ҷ��� ������: " + snapshot.Value);
            }
        });
    }

    void SaveData(string userName, int userScore)
    {
        dbReference.Child("users").Child("user1").Child("name").SetValueAsync(userName);
        dbReference.Child("users").Child("user1").Child("score").SetValueAsync(userScore);
    }

}
