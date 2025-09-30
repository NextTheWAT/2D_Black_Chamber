using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpawner : MonoBehaviour
{
    public List<Transform> spawnerPoint;

    public GameObject coinPrefab;
    public GameObject billPrefab;
    public GameObject tripleBillPrefab;


    public int stageMaxTotal = 500;
    public int spawnCount = 5;

    public int minValue = 50;
    public int maxValue = 300;

    private void Start()
    {
        SpawnMoney();
    }

    private void SpawnMoney()
    {
        if (spawnerPoint == null || spawnerPoint.Count == 0)
            return;

        List<Transform> spawnPoint = new List<Transform>(spawnerPoint); // 스폰 포인트
        List<Transform> choosePoint = new List<Transform>();            // 선택된 스폰 포인트
        for (int i = 0; i < spawnCount; i++)
        {
            // 리스트에 있는 스폰포인트들 중 랜덤 스폰카운트개수만큼 선택
            int randomIndex = Random.Range(0, spawnPoint.Count);
            choosePoint.Add(spawnPoint[randomIndex]);     // 선택한 위치 저장
            spawnPoint.RemoveAt(randomIndex);             // 중복 방지를 위해 제거 (뽑은거 또 못뽑게)

        }


        int totalMoney = stageMaxTotal;
        int[] amounts = new int[spawnCount];    // 스폰카운트 배열
        for (int i = 0; i < spawnCount - 1; i++)
        {
            int maxMoney = Mathf.Min(maxValue, totalMoney - minValue * (spawnCount - i - 1));
            int amount = Random.Range(minValue, maxMoney + 1);
            amounts[i] = amount;
            totalMoney -= amount;
        }


        amounts[spawnCount -1] = totalMoney;        // 마지막 머니프리펩 = 나머지 모든 금액

        // 프리팹을 생성
        for (int i = 0; i < spawnCount; i++)
        {
            int amount = amounts[i];
            GameObject prefab;

            // 금액에따른 프리팹이 생성되게 구분
            if (amount >= stageMaxTotal)
                prefab = tripleBillPrefab;
            else if (amount <= stageMaxTotal / 2f)
                prefab = coinPrefab;
            else
                prefab = billPrefab;

            GameObject moneyPrefabs = Instantiate(prefab, choosePoint[i].position, Quaternion.identity);

            Money moneyComponent = moneyPrefabs.GetComponent<Money>();          // 생성한 머니프리펩의 컴포넌트를 머니로 가져가기 (금액 정보)
            if ( moneyComponent != null)
                moneyComponent.SetAmount(amount);
        }

        int debugTotal = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            debugTotal += amounts[i];
            Debug.Log($"프리팹 {i + 1} 금액: {amounts[i]}");
        }
        Debug.Log($"총 생성 금액 합계: {debugTotal}");
    }
}
