using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public GameObject enemySpawnerPrefab; // 적 스포너 프리팹

    public float spawnInterval = 20f; // 적 생성 간격
    public int MaxSpawnEnemyCount { get; private set; } = 0; // 최대 생성 가능한 적 수
    public int ActiveEnemyCount { get; private set; } = 0; // 현재 활성화된 적 수


    public void SpawnSpawner()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            IncreaseEnemyCount();
            Instantiate(enemySpawnerPrefab, enemy.transform.position, Quaternion.identity);
        }
    }

    public void ResetEnemyCount()
    {
        ActiveEnemyCount = 0;
        MaxSpawnEnemyCount = 0;
    }

    public void IncreaseEnemyCount()
    {
        ActiveEnemyCount++;
        if (ActiveEnemyCount > MaxSpawnEnemyCount)
            MaxSpawnEnemyCount = ActiveEnemyCount;
    }

    public void DecreaseEnemyCount()
    {
        ActiveEnemyCount--;
        if (ActiveEnemyCount < 0)
            ActiveEnemyCount = 0;
    }



}
