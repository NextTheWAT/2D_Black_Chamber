using System;
using UnityEngine;
using Constants;
using System.Collections;


public class GameManager : Singleton<GameManager>
{
    public Transform player;
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

    protected override void Initialize()
    {
        base.Initialize();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
