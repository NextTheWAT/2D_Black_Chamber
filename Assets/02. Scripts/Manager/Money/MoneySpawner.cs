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

        List<Transform> spawnPoint = new List<Transform>(spawnerPoint); // ���� ����Ʈ
        List<Transform> choosePoint = new List<Transform>();            // ���õ� ���� ����Ʈ
        for (int i = 0; i < spawnCount; i++)
        {
            // ����Ʈ�� �ִ� ��������Ʈ�� �� ���� ����ī��Ʈ������ŭ ����
            int randomIndex = Random.Range(0, spawnPoint.Count);
            choosePoint.Add(spawnPoint[randomIndex]);     // ������ ��ġ ����
            spawnPoint.RemoveAt(randomIndex);             // �ߺ� ������ ���� ���� (������ �� ���̰�)

        }


        int totalMoney = stageMaxTotal;
        int[] amounts = new int[spawnCount];    // ����ī��Ʈ �迭
        for (int i = 0; i < spawnCount - 1; i++)
        {
            int maxMoney = Mathf.Min(maxValue, totalMoney - minValue * (spawnCount - i - 1));
            int amount = Random.Range(minValue, maxMoney + 1);
            amounts[i] = amount;
            totalMoney -= amount;
        }


        amounts[spawnCount -1] = totalMoney;        // ������ �Ӵ������� = ������ ��� �ݾ�

        // �������� ����
        for (int i = 0; i < spawnCount; i++)
        {
            int amount = amounts[i];
            GameObject prefab;

            // �ݾ׿����� �������� �����ǰ� ����
            if (amount >= stageMaxTotal)
                prefab = tripleBillPrefab;
            else if (amount <= stageMaxTotal / 2f)
                prefab = coinPrefab;
            else
                prefab = billPrefab;

            GameObject moneyPrefabs = Instantiate(prefab, choosePoint[i].position, Quaternion.identity);

            Money moneyComponent = moneyPrefabs.GetComponent<Money>();          // ������ �Ӵ��������� ������Ʈ�� �ӴϷ� �������� (�ݾ� ����)
            if ( moneyComponent != null)
                moneyComponent.SetAmount(amount);
        }

        int debugTotal = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            debugTotal += amounts[i];
            Debug.Log($"������ {i + 1} �ݾ�: {amounts[i]}");
        }
        Debug.Log($"�� ���� �ݾ� �հ�: {debugTotal}");
    }
}
