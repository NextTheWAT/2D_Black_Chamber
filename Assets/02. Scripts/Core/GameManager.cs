using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Constants;
using System.Collections.Generic;
using Esper.Freeloader;

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

    public LayerMask obstacleLayerMask;
    public LayerMask enemyLayerMask;

    public bool isLoadingEnabled = false;

    public Transform Player
    {
        get
        {
            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                player = playerObject ? playerObject.transform : null;
            }

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
            }
        }
    }

    private void OnEnable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadingScreen.Instance.onStart.AddListener(LoadingStart);
        LoadingScreen.Instance.onClose.AddListener(InvokeLoadingClose);
    }

    private void OnDisable()
    {
        if (AppIsQuitting) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LoadingScreen.Instance.onStart.RemoveListener(LoadingStart);
        LoadingScreen.Instance.onClose.RemoveListener(InvokeLoadingClose);
    }

    private void LoadingStart()
        => isLoadingEnabled = true;
    private void LoadingClose()
        => isLoadingEnabled = false;

    // 약간의 지연을 두고 로딩 종료 처리 (총 바로 못쏘도록 설정)
    private void InvokeLoadingClose()
        => Invoke(nameof(LoadingClose), 0.1f);

    private void OnLoading(bool isLoading)
    {
        if (Player == null) return;
        Player.gameObject.SetActive(!isLoading);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsCombat = false;
        OnPhaseChanged?.Invoke(CurrentPhase);
        targetFoundEnemies.Clear();

        if (enterCombatCoroutine != null)
            StopCoroutine(enterCombatCoroutine);

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
