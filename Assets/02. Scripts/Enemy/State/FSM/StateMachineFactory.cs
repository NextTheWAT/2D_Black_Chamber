using Constants;
using System;

public static class StateMachineFactory
{
    public static StateMachine CreateStateMachine(Enemy owner, StateTable stateTable, Type startType, NonCombatStateType nonCombatStateType)
    {
        StateMachine stateMachine = new(owner, stateTable, startType);

        InvestigateState investigateState = stateMachine.GetState<InvestigateState>();

        // ����
        stateMachine.AddGlobalTransition<DeathState>(() => owner.IsDead); // ���
        stateMachine.AddGlobalTransition<CoverState>(() => owner.IsHit && stateMachine.CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // �¾��� �� �ǽɻ��¸� �߰� �� ���� ����
        stateMachine.AddGlobalTransition<InvestigateState>(()=> owner.IsNoiseDetected);


        switch (nonCombatStateType)
        {
            case NonCombatStateType.Common:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ǽ�
                stateMachine.AddTransition<SuspectState, InvestigateState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<InvestigateState, SuspectState>(() => owner.HasTargetInFOV); // Ÿ�� �߰��ϸ� �ǽ�
                stateMachine.AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // ���� �ð� ������ ����
                stateMachine.AddTransition<ReturnState, PatrolState>(() => owner.IsArrived); // ��ȯ �Ϸ�� ����
                break;

            case NonCombatStateType.Elite:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ǽ�
                stateMachine.AddTransition<SuspectState, InvestigateState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<InvestigateState, SuspectState>(() => owner.HasTargetInFOV); // Ÿ�� �߰��ϸ� �ǽ�
                stateMachine.AddTransition<InvestigateState, PatrolState>(() => !investigateState.IsInvestigating); // ���� �ð� ������ �ٽ� ����
                break;

            case NonCombatStateType.Guard:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ǽ�
                stateMachine.AddTransition<SuspectState, PatrolState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ٽ� ����
                break;
        }

        return stateMachine;
    }

    public static StateMachine CreatetStateMachine(Enemy owner, StateTable stateTable, Type startType, CombatStateType combatStateType)
    {
        StateMachine stateMachine = new(owner, stateTable, startType);

        AssaultState assaultState = stateMachine.GetState<AssaultState>();
        AttackState attackState = stateMachine.GetState<AttackState>();
        RetreatState retreatState = stateMachine.GetState<RetreatState>();

        // ����
        stateMachine.AddGlobalTransition<DeathState>(() => owner.IsDead); // ���
        stateMachine.AddGlobalTransition<CoverState>(() => owner.IsHit && stateMachine.CurrentState.GetType() != typeof(AttackState) && stateMachine.CurrentState.GetType() != typeof(RetreatState)); // �¾��� �� ���ݻ��°� �ƴϸ� ����

        switch (combatStateType)
        {
            case CombatStateType.Hiding: // ����
                stateMachine.AddTransition<CoverState, AttackState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AttackState, CoverState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ٽ� ����
                break;

            case CombatStateType.Discretion: // ����
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.IsTargetInSight); // Ÿ���� �þ߿� ������ ����
                stateMachine.AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange); // �����Ÿ� ����� ����
                stateMachine.AddTransition<AttackState, RetreatState>(() => retreatState.ShouldRetreat); // ü�� ������ ����
                stateMachine.AddTransition<RetreatState, CoverState>(() => !retreatState.IsRetreating); // ���� �ð� ������ �ٽ� ����
                break;

            case CombatStateType.Tactics: // ����
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange); // �����Ÿ� ����� ����
                break;

            case CombatStateType.Bravery: // �밨
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AttackState, AssaultState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ٽ� ����
                break;

            case CombatStateType.Temerity: // ����
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<AttackState, AssaultState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ٽ� ����
                break;

            case CombatStateType.Coward: // ������
                stateMachine.AddTransition<CoverState, AttackState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
                stateMachine.AddTransition<CoverState, RetreatState>(() => retreatState.ShouldRetreat); // ü�� ������ ����
                stateMachine.AddTransition<AttackState, RetreatState>(() => retreatState.ShouldRetreat); // ü�� ������ ����
                stateMachine.AddTransition<RetreatState, CoverState>(() => !retreatState.IsRetreating); // ���� �ð� ������ �ٽ� ����
                break;
        }


        return stateMachine;
    }
}
