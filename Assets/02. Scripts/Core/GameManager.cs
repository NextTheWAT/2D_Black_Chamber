using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Constants;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public event Action<GamePhase> OnPhaseChanged;
    public float combatDuration = 5f; // 전투 상태 지속 시간
    public float combatDelay = 2f;
    public string gameOverSceneName = "GameOverScene";

    public GamePhase CurrentPhase { get; set; } = GamePhase.Stealth;
    private Transform player;

    private Coroutine enterCombatCoroutine;
    private HashSet<Enemy> targetFoundEnemies = new();


    public Transform Player
    {
        get
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player").transform;

            return player;
        }
    }

    public bool IsCombat
    {
        get => CurrentPhase == GamePhase.Combat;
        set
        {
            GamePhase next = value ? GamePhase.Combat : GamePhase.Stealth;
            if (CurrentPhase != next)
            {
                CurrentPhase = next;
                OnPhaseChanged?.Invoke(CurrentPhase); //총 UI 변경 이벤트 발행
                WeaponManager.Instance.CurrentWeaponIndex = 0;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsCombat = false;
        OnPhaseChanged?.Invoke(CurrentPhase);
        targetFoundEnemies.Clear();
        enterCombatCoroutine = null;
    }

    public void StartCombatAfterDelay(Enemy enemy)
    {
        targetFoundEnemies.Add(enemy);
        if (enterCombatCoroutine != null) return;
        enterCombatCoroutine = StartCoroutine(EnterCombatAfterDelay());
    }

    public void CancelCombatDelay(Enemy enemy)
    {
        targetFoundEnemies.Remove(enemy);

        if (targetFoundEnemies.Count > 0) return;
        if (enterCombatCoroutine == null) return;
        StopCoroutine(enterCombatCoroutine);
        enterCombatCoroutine = null;
    }

    IEnumerator EnterCombatAfterDelay()
    {
        yield return new WaitForSeconds(combatDelay);
        IsCombat = true;
    }

    public void TriggerGameOver()
        => Invoke(nameof(LoadGameOverScene), 2f);

    private void LoadGameOverScene()
        => SceneManager.LoadScene(gameOverSceneName);

}
