using System;
using UnityEngine;

public class MissionManager : Singleton<MissionManager>
{
    [Header("Runtime")]
    [SerializeField] private int remainingTargets = 0;
    [SerializeField] private int remainingEnemies = 0;
    [SerializeField] private MissionPhase phase = MissionPhase.Assassination;

    public int RemainingTargets => remainingTargets;
    public int RemainingEnemies => remainingEnemies;
    public MissionPhase Phase => phase;

    public event Action<int> OnTargetsChanged;
    public event Action<int> OnEnemiesChanged;
    public event Action<MissionPhase> OnPhaseChanged;

    // --- �ϻ� ��� ---
    public void TargetActivated()
    {
        remainingTargets++;
        OnTargetsChanged?.Invoke(remainingTargets);
        EvaluatePhase();
    }

    public void TargetDeactivated()
    {
        if (remainingTargets > 0) remainingTargets--;
        OnTargetsChanged?.Invoke(remainingTargets);
        EvaluatePhase();
    }

    // --- �� ---
    public void EnemyActivated()
    {
        remainingEnemies++;
        OnEnemiesChanged?.Invoke(remainingEnemies);
    }

    public void EnemyDeactivated()
    {
        if (remainingEnemies > 0) remainingEnemies--;
        OnEnemiesChanged?.Invoke(remainingEnemies);
    }

    // �ϻ� ����� 0�̸� ���� �ܰ��
    private void EvaluatePhase()
    {
        if (phase == MissionPhase.Assassination && remainingTargets <= 0)
            SetPhase(MissionPhase.Escape);
    }

    public void SetPhase(MissionPhase next)
    {
        if (phase == next) return;
        phase = next;
        OnPhaseChanged?.Invoke(phase);
#if UNITY_EDITOR
        Debug.Log($"[MissionManager] Phase -> {phase}");
#endif
    }

    // ����: �ʱ�ȭ�� �ʿ��� ��
    public void ResetCounts(int targets = 0, int enemies = 0, MissionPhase startPhase = MissionPhase.Assassination)
    {
        remainingTargets = Mathf.Max(0, targets);
        remainingEnemies = Mathf.Max(0, enemies);
        SetPhase(startPhase);
        OnTargetsChanged?.Invoke(remainingTargets);
        OnEnemiesChanged?.Invoke(remainingEnemies);
    }
}
