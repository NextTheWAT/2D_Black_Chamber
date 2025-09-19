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
        AssaultState assaultState = GetState<AssaultState>();
        AttackState attackState = GetState<AttackState>();
        // ����

        // Global
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // �¾��� �� �ǽɻ��¸� �߰� �� ���� ����
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState)); // �¾��� �� ���ݻ��°� �ƴϸ� �߰�

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ǽ�
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� ����
        AddTransition<PatrolState, CoverState>(() => GameManager.Instance.IsCombat); // ���� ���۵Ǹ� ����


        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ٽ� ����
        AddTransition<SuspectState, CoverState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // Ÿ�� �߰��ϸ� �߰�, ��������
        AddTransition<SuspectState, CoverState>(() => GameManager.Instance.IsCombat); // ���� ���۵Ǹ� ����

        // Investigate
        AddTransition<InvestigateState, SuspectState>(() => owner.HasSuspiciousTarget); // Ÿ�� �߰��ϸ� �߰�
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // ���� �ð� ������ ����

        // Return
        AddTransition<ReturnState, ChaseState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

        // ����

        // Chase
        AddTransition<ChaseState, CoverState>(() => chaseState.IsTargetInChaseRange); // �����Ÿ� �ȿ� ������ ����

        // Cover
        // AddTransition<CoverState, ChaseState>(() => !chaseState.IsTargetInChaseRange); // �����Ÿ� ����� �߰�
        AddTransition<CoverState, AttackState>(() => attackState.IsTargetInAttackRange && owner.IsTargetInSight); // �����Ÿ� �ȿ� ������ ����

        // Assault
        AddTransition<AssaultState, ChaseState>(() => !chaseState.IsTargetInChaseRange); // �����Ÿ� ����� �߰�
        AddTransition<AssaultState, AttackState>(() => owner.IsTargetInSight); // ���� �����ϸ� ����

        // Attack
        AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange || !owner.IsTargetInSight); // �����Ÿ� ����� �߰�

    }
}
