using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public GameObject spawnPrefab; // 적 프리팹
    public int spawnCount = 5; // 한 번에 생성할 적 수
    public float spawnInterval = 40f; // 적 생성 간격
    public int MaxSpawnEnemyCount { get; private set; } = 0; // 최대 생성 가능한 적 수
    public int ActiveEnemyCount { get; private set; } = 0; // 현재 활성화된 적 수

    private List<Vector2> spawnPositions = new();
    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        if (GameManager.AppIsQuitting) return;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDisable()
    {
        if (GameManager.AppIsQuitting) return;
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    public void OnPhaseChanged(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.Combat)
            StartSpawn();
    }


    public void Setting()
    {
        StopSpawn();
        spawnPositions.Clear();

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        ActiveEnemyCount = enemies.Length;
        MaxSpawnEnemyCount = enemies.Length;

        foreach (Enemy enemy in enemies)
            spawnPositions.Add(enemy.transform.position);
    }

    void StartSpawn()
    {
        if (spawnCoroutine != null) return;
        spawnCoroutine = StartCoroutine(Spawn());
    }

    void StopSpawn()
    {
        if(spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = null;
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            int count = 0;
            spawnPositions.Shuffle();
            Debug.Log($"[SpawnManager] Spawn Start. ActiveEnemyCount: {ActiveEnemyCount}, MaxSpawnEnemyCount: {MaxSpawnEnemyCount}");

            foreach (var spawnPosition in spawnPositions)
            {
                // 적 개수가 최대치 이상이면 중단
                bool canSpawnEnemy = MaxSpawnEnemyCount > ActiveEnemyCount;
                if (!canSpawnEnemy) break;

                // 화면에 보이면 넘어감
                bool isVisible = VisibilityUtility.IsVisible(spawnPosition);
                if (isVisible) continue;

                // 무언가에 안가려져 있으면 넘어감
                bool linecastHit = Physics2D.Linecast(spawnPosition, GameManager.Instance.Player.position, GameManager.Instance.obstacleLayerMask);
                if(!linecastHit) continue;

                // 적 생성
                Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

                // 생성 개수가 최대치면 중단
                if(++count >= spawnCount) break;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
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
