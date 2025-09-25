using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Constants;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public event Action<GamePhase> OnPhaseChanged;
    public float combatDuration = 5f; // ���� ���� ���� �ð�
    public float combatDelay = 2f;
    public string gameOverSceneName = "GameOverScene";

    public GamePhase CurrentPhase { get; set; } = GamePhase.Stealth;
    private Transform player;

    private float exitCombatTime;
    private Coroutine enterCombatCoroutine;
    private Coroutine exitCombatCoroutine;
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
                OnPhaseChanged?.Invoke(CurrentPhase); //�� UI ���� �̺�Ʈ ����
                WeaponManager.Instance.Toggle();

                if (CurrentPhase == GamePhase.Combat)
                {
                    RefreshCombatTimer();
                    exitCombatCoroutine ??= StartCoroutine(ExitCombatAfterDelay());
                    ConditionalLogger.Log("Entered Combat Mode");
                }
                else
                {
                    if (exitCombatCoroutine != null)
                    {
                        StopCoroutine(exitCombatCoroutine);
                        exitCombatCoroutine = null;
                    }
                }
            }
        }
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
        if(enterCombatCoroutine == null) return;
        StopCoroutine(enterCombatCoroutine);
        enterCombatCoroutine = null;
    }

    IEnumerator EnterCombatAfterDelay()
    {
        yield return new WaitForSeconds(combatDelay);
        IsCombat = true;
    }

    IEnumerator ExitCombatAfterDelay()
    {
        while (Time.time < exitCombatTime)
            yield return null;
        IsCombat = false;
        exitCombatCoroutine = null;
        ConditionalLogger.Log("Exited Combat Mode");
    }

    public void RefreshCombatTimer()
    {
        if (IsCombat)
            exitCombatTime = Time.time + combatDuration;
    }

    public void TriggerGameOver()
        => Invoke(nameof(LoadGameOverScene), 2f);

    private void LoadGameOverScene()
        => SceneManager.LoadScene(gameOverSceneName);

}
