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
            // ����Ʈ�� �ִ� ��������Ʈ�� �� ���� ����ī��Ʈ������ŭ ����
            int randomIndex = Random.Range(i, randomList.Count);
            Transform random = randomList[i];
            randomList[i] = randomList[randomIndex];
            randomList[randomIndex] = random;
        }
        // �������� ����
        for (int i = 0; i < Mathf.Min(spawnCount,randomList.Count); i++)
        {
            Transform random = randomList[i];

            int amount = Random.Range(minValue, Mathf.Min(maxValue, stageMaxTotal) + 1);    // + 1 �ؾ� 300����

            amount = Mathf.Min(amount, stageMaxTotal);

            // �ݾ׿����� �������� �����ǰ� ����
            GameObject prefab;
            if (amount >= stageMaxTotal)
                prefab = tripleBillPrefab;
            else if (amount <= stageMaxTotal / 2f)
                prefab = coinPrefab;
            else
                prefab = billPrefab;

            GameObject moneyPrefabs = Instantiate(prefab, random.position, Quaternion.identity);
            Money moneyComponent = moneyPrefabs.GetComponent<Money>();          // ������ �Ӵ��������� ������Ʈ�� �ӴϷ� �������� (�ݾ� ����)

            if ( moneyComponent != null)
                moneyComponent.SetAmount(amount);

            stageMaxTotal -= amount;
        }

    }
}
