using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseLoader : MonoBehaviour
{
    public static Dictionary<int, EnemyData> enemyDataDict = new Dictionary<int, EnemyData>();
    private const string firebaseUrl = "https://blackchamber-f4f4a-default-rtdb.firebaseio.com/EnemyData.json";

    void Start()
    {
        StartCoroutine(LoadDataCoroutine());
    }

    public static EnemyData GetEnemyData(int id)
    {
        return enemyDataDict.ContainsKey(id) ? enemyDataDict[id] : null;
    }

    private IEnumerator LoadDataCoroutine()
    {
        ConditionalLogger.Log("Firebase 데이터 로드 시작...");

        using (UnityWebRequest request = UnityWebRequest.Get(firebaseUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ConditionalLogger.LogError($"Firebase 데이터 로드 실패: {request.error}");
            }
            else
            {
                string json = request.downloadHandler.text;
                ConditionalLogger.Log("Raw JSON: " + json);

                enemyDataDict = LoadEnemiesAsDictionary(json);

                Debug.Log("Enemy count: " + enemyDataDict.Count);
                foreach (var kvp in enemyDataDict)
                {
                    ConditionalLogger.Log($"ID: {kvp.Key}, Name: {kvp.Value.enemyName}");
                }

                ConditionalLogger.Log("Firebase 데이터 로드 완료!");
            }
        }
    }

    Dictionary<int, EnemyData> LoadEnemiesAsDictionary(string json)
    {
        Dictionary<int, EnemyData> dict = new Dictionary<int, EnemyData>();

        // 1. 최상위 { } 제거
        string trimmed = json.Trim();
        if (trimmed.StartsWith("{")) trimmed = trimmed.Substring(1);
        if (trimmed.EndsWith("}")) trimmed = trimmed.Substring(0, trimmed.Length - 1);

        // 2. 최상위 아이템 분리
        int braceCount = 0;
        int startIndex = 0;
        List<string> entries = new List<string>();

        for (int i = 0; i < trimmed.Length; i++)
        {
            if (trimmed[i] == '{') braceCount++;
            else if (trimmed[i] == '}') braceCount--;

            if (braceCount == 0 && trimmed[i] == '}')
            {
                entries.Add(trimmed.Substring(startIndex, i - startIndex + 1));
                startIndex = i + 2; // }, 뒤에 오는 콤마 건너뛰기
            }
        }

        // 3. 각 아이템 파싱
        foreach (var entry in entries)
        {
            int colonIndex = entry.IndexOf(':');
            if (colonIndex < 0) continue;

            // 키(ID) 추출
            string keyStr = entry.Substring(0, colonIndex).Trim().Replace("\"", "");
            int id = int.Parse(keyStr);

            // 값(JSON) 추출
            string valueJson = entry.Substring(colonIndex + 1).Trim();

            // EnemyData로 변환
            EnemyData data = JsonUtility.FromJson<EnemyData>(valueJson);
            dict.Add(id, data);
        }

        return dict;
    }
}
