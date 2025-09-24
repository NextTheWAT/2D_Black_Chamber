using System;
using UnityEngine;
using Constants;
using System.Collections;


public class GameManager : Singleton<GameManager>
{
    private Transform player;
    public Transform Player
    {
        get
        {
            if(player == null)
                player = GameObject.FindGameObjectWithTag("Player").transform;

            return player;
        }
    }
    public float combatDuration = 5f; // 전투 상태 지속 시간


    public GamePhase CurrentPhase { get; set; } = GamePhase.Stealth;

    public event Action<GamePhase> OnPhaseChanged;

    private Coroutine exitCombatCoroutine;
    private float exitCombatTime;
    

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
                WeaponManager.Instance.Toggle();

                if (CurrentPhase == GamePhase.Combat)
                {
                    RefreshCombatTimer();
                    exitCombatCoroutine ??= StartCoroutine(ExitCombat());
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

    IEnumerator ExitCombat()
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

}
