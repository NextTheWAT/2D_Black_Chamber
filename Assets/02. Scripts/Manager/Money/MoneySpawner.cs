using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpawner : MonoBehaviour
{
    public List<MoneySpawnZone> spawnerPoint;

    public GameObject coinPrefab;
    public GameObject billPrefab;
    public GameObject tripleBillPrefab;


    public int stageMaxTotal = 500;
    public int spawnCount = 5;

    private void Start()
    {
        SpawnMoney();
    }

    private void SpawnMoney()
    {
        if (spawnerPoint == null || spawnerPoint.Count == 0)
            return;

        List<MoneySpawnZone> randomList = new List<MoneySpawnZone>(spawnerPoint);
        for (int i = 0; i < spawnerPoint.Count; i++)
        {
            // 리스트에 있는 스폰포인트들 중 랜덤 스폰카운트개수만큼 선택
            MoneySpawnZone random = randomList[i];
            int randomIndex = Random.Range(i, randomList.Count);
            randomList[i] = randomList[randomIndex];
            randomList[randomIndex] = random;
            // 프리팹을 생성

            // 금액에따른 프리팹이 생성되게 구분
        }
    }
}
