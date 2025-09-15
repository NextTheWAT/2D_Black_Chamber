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
        AddGlobalTransition<ChaseState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // �¾��� �� �ǽɻ��¸� �߰� �� ���� ����
        AddGlobalTransition<ChaseState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState), ()=> owner.LastKnownTargetPos = GameManager.Instance.player.position); // �¾��� �� ���ݻ��°� �ƴϸ� �߰�

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ǽ�
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� ����
        AddTransition<PatrolState, InvestigateState>(() => !owner.HasTarget && GameManager.Instance.IsCombat); // Ÿ�� ���� �������¸� ����

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ٽ� ����
        AddTransition<SuspectState, ChaseState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // Ÿ�� �߰��ϸ� �߰�, ��������
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasTarget && GameManager.Instance.IsCombat); // Ÿ�� ���� �������¸� ����

        // Chase
        AddTransition<ChaseState, AttackState>(() => attackState.IsTargetInAttackRange && GameManager.Instance.IsCombat); // �����Ÿ� �ȿ� ������ ����
        AddTransition<ChaseState, SuspectState>(() => owner.HasTarget && !GameManager.Instance.IsCombat); // �����Ÿ� �ȿ� ������ ����
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
