using Constants;
using System;

public static class StateMachineFactory
{
    public static StateMachine CreateStateMachine(Enemy owner, StateTable stateTable, Type startType, NonCombatStateType nonCombatStateType)
    {
        StateMachine stateMachine = new(owner, stateTable, startType);

        InvestigateState investigateState = stateMachine.GetState<InvestigateState>();

        // 공용
        stateMachine.AddGlobalTransition<DeathState>(() => owner.IsDead); // 사망
        stateMachine.AddGlobalTransition<CoverState>(() => owner.IsHit && stateMachine.CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // 맞았을 때 의심상태면 추격 및 난전 시작
        stateMachine.AddGlobalTransition<InvestigateState>(()=> owner.IsNoiseDetected);


        switch (nonCombatStateType)
        {
            case NonCombatStateType.Common:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 의심
                stateMachine.AddTransition<SuspectState, InvestigateState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 수색
                stateMachine.AddTransition<InvestigateState, SuspectState>(() => owner.HasTargetInFOV); // 타겟 발견하면 의심
                stateMachine.AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // 조사 시간 끝나면 복귀
                stateMachine.AddTransition<ReturnState, PatrolState>(() => owner.IsArrived); // 귀환 완료시 순찰
                break;

            case NonCombatStateType.Elite:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 의심
                stateMachine.AddTransition<SuspectState, InvestigateState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 수색
                stateMachine.AddTransition<InvestigateState, SuspectState>(() => owner.HasTargetInFOV); // 타겟 발견하면 의심
                stateMachine.AddTransition<InvestigateState, PatrolState>(() => !investigateState.IsInvestigating); // 조사 시간 끝나면 다시 순찰
                break;

            case NonCombatStateType.Guard:
                stateMachine.AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 의심
                stateMachine.AddTransition<SuspectState, PatrolState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 다시 순찰
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

        // 공용
        stateMachine.AddGlobalTransition<DeathState>(() => owner.IsDead); // 사망
        stateMachine.AddGlobalTransition<CoverState>(() => owner.IsHit && stateMachine.CurrentState.GetType() != typeof(AttackState) && stateMachine.CurrentState.GetType() != typeof(RetreatState)); // 맞았을 때 공격상태가 아니면 엄폐

        switch (combatStateType)
        {
            case CombatStateType.Hiding: // 은신
                stateMachine.AddTransition<CoverState, AttackState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AttackState, CoverState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 다시 엄폐
                break;

            case CombatStateType.Discretion: // 신중
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.IsTargetInSight); // 타겟이 시야에 들어오면 공격
                stateMachine.AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange); // 사정거리 벗어나면 엄폐
                stateMachine.AddTransition<AttackState, RetreatState>(() => retreatState.ShouldRetreat); // 체력 낮으면 후퇴
                stateMachine.AddTransition<RetreatState, CoverState>(() => !retreatState.IsRetreating); // 후퇴 시간 지나면 다시 엄폐
                break;

            case CombatStateType.Tactics: // 전술
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange); // 사정거리 벗어나면 엄폐
                break;

            case CombatStateType.Bravery: // 용감
                stateMachine.AddTransition<CoverState, AssaultState>(() => attackState.IsTargetInAttackRange); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AttackState, AssaultState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 다시 돌격
                break;

            case CombatStateType.Temerity: // 무모
                stateMachine.AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<AttackState, AssaultState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 다시 돌격
                break;

            case CombatStateType.Coward: // 겁쟁이
                stateMachine.AddTransition<CoverState, AttackState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 공격
                stateMachine.AddTransition<CoverState, RetreatState>(() => retreatState.ShouldRetreat); // 체력 낮으면 후퇴
                stateMachine.AddTransition<AttackState, RetreatState>(() => retreatState.ShouldRetreat); // 체력 낮으면 후퇴
                stateMachine.AddTransition<RetreatState, CoverState>(() => !retreatState.IsRetreating); // 후퇴 시간 지나면 다시 엄폐
                break;
        }


        return stateMachine;
    }
}
