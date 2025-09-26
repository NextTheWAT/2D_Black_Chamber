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

        List<Transform> randomList = new List<Transform>(spawnerPoint);
        for (int i = 0; i < spawnerPoint.Count; i++)
        {
            // 리스트에 있는 스폰포인트들 중 랜덤 스폰카운트개수만큼 선택
            int randomIndex = Random.Range(i, randomList.Count);
            Transform random = randomList[i];
            randomList[i] = randomList[randomIndex];
            randomList[randomIndex] = random;
        }
        // 프리팹을 생성
        for (int i = 0; i < Mathf.Min(spawnCount,randomList.Count); i++)
        {
            Transform random = randomList[i];

            int amount = Random.Range(minValue, Mathf.Min(maxValue, stageMaxTotal) + 1);    // + 1 해야 300이하

            amount = Mathf.Min(amount, stageMaxTotal);

            // 금액에따른 프리팹이 생성되게 구분
            GameObject prefab;
            if (amount >= stageMaxTotal)
                prefab = tripleBillPrefab;
            else if (amount <= stageMaxTotal / 2f)
                prefab = coinPrefab;
            else
                prefab = billPrefab;

            GameObject moneyPrefabs = Instantiate(prefab, random.position, Quaternion.identity);
            Money moneyComponent = moneyPrefabs.GetComponent<Money>();          // 생성한 머니프리펩의 컴포넌트를 머니로 가져가기 (금액 정보)

            if ( moneyComponent != null)
                moneyComponent.SetAmount(amount);

            stageMaxTotal -= amount;
        }

    }
}
