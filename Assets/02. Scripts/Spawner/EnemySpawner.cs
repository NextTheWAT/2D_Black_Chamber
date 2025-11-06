using System.Collections;
using UnityEngine;
using Constants;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private VisibilityChecker visibilityChecker;
    private bool isSpawningInProgress = false; // 생성 중복 방지

    private void OnEnable()
        => GameManager.Instance.OnPhaseChanged += OnPhaseChanged;

    private void OnDisable()
        => GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;

    public void OnPhaseChanged(GamePhase gamePhase)
    {
        visibilityChecker = GetComponent<VisibilityChecker>();

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
            // 무언가에 가려졌을 때만 생성
            if (Physics2D.Linecast(transform.position, GameManager.Instance.Player.position, GameManager.Instance.obstacleLayerMask))
                // 화면에 보이지 않을 때만 적 생성
                if (SpawnManager.Instance.MaxSpawnEnemyCount > SpawnManager.Instance.ActiveEnemyCount && !visibilityChecker.Visible)
                    Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(SpawnManager.Instance.spawnInterval);
        }
    }


}
