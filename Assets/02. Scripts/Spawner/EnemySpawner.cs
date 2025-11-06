using System.Collections;
using UnityEngine;
using Constants;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private bool isSpawningInProgress = false; // 생성 중복 방지

    private void OnEnable()
        => GameManager.Instance.OnPhaseChanged += OnPhaseChanged;

    private void OnDisable()
        => GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;

    public void OnPhaseChanged(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.Combat)
            StartSpawn();
    }

    public void StartSpawn()
    {
        if (enemyPrefab == null) return;
        if (isSpawningInProgress) return;

        isSpawningInProgress = true;
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (SpawnManager.Instance.MaxSpawnEnemyCount > SpawnManager.Instance.ActiveEnemyCount)
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(SpawnManager.Instance.spawnInterval);
        }
    }
}
