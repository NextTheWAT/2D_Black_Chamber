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
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget);

        // Chase
        AddTransition<ChaseState, AttackState>(() => attackState.IsTargetInAttackRange);
        AddTransition<ChaseState, InvestigateState>(() => !chaseState.IsChasing);

        // Attack
        AddTransition<AttackState, InvestigateState>(() => !owner.HasTarget);

        // Investigate
        AddTransition<InvestigateState, ChaseState>(() => owner.HasTarget);
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating);

        // Return
        AddTransition<ReturnState, ChaseState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

    }
}
