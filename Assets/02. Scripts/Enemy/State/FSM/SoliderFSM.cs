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
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ǽ�
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� ����

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ٽ� ����
        AddTransition<SuspectState, ChaseState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� �߰�

        // Chase
        AddTransition<ChaseState, AttackState>(() => attackState.IsTargetInAttackRange); // �����Ÿ� �ȿ� ������ ����
        AddTransition<ChaseState, InvestigateState>(() => !chaseState.IsChasing); // Ÿ�� ������ ����

        // Attack
        AddTransition<AttackState, InvestigateState>(() => !owner.HasTarget); // Ÿ�� ������ ����

        // Investigate
        AddTransition<InvestigateState, ChaseState>(() => owner.HasTarget);
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating);

        // Return
        AddTransition<ReturnState, ChaseState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

    }
}
