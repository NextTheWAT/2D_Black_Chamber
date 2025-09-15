using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class SoliderFSM : StateMachine
{
    public SoliderFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        InvestigateState investigateState = GetState<InvestigateState>();
        ChaseState chaseState = GetState<ChaseState>();
        AttackState attackState = GetState<AttackState>();

        // Global
        AddGlobalTransition<InvestigateState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState));

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // 근처에 타겟 있으면 의심
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // 타겟 발견하면 공격

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, ChaseState>(() => owner.HasTarget); // 타겟 발견하면 추격

        // Chase
        AddTransition<ChaseState, AttackState>(() => attackState.IsTargetInAttackRange); // 사정거리 안에 들어오면 공격
        AddTransition<ChaseState, InvestigateState>(() => !chaseState.IsChasing); // 타겟 잃으면 수색

        // Attack
        AddTransition<AttackState, InvestigateState>(() => !owner.HasTarget); // 타겟 잃으면 수색

        // Investigate
        AddTransition<InvestigateState, ChaseState>(() => owner.HasTarget);
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating);

        // Return
        AddTransition<ReturnState, ChaseState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

    }
}
