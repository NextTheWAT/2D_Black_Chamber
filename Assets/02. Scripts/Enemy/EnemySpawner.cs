using System.Collections;
using UnityEngine;
using Constants;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public int spawnCount = 5;            // ������ ��
    public float spawnInterval = 2f;      // ���� ����
    private bool isSpawningInProgress = false; // ���� �ߺ� ����

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
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
