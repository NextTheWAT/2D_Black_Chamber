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
        if (spawnPoint.Count < spawnCount) spawnCount = spawnerPoint.Count;
    }
}
