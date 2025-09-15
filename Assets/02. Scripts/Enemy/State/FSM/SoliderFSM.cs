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
        AddGlobalTransition<ChaseState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // 맞았을 때 의심상태면 추격 및 난전 시작
        AddGlobalTransition<ChaseState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState), ()=> owner.LastKnownTargetPos = GameManager.Instance.player.position); // 맞았을 때 공격상태가 아니면 추격

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // 근처에 타겟 있으면 의심
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // 타겟 발견하면 공격
        AddTransition<PatrolState, InvestigateState>(() => !owner.HasTarget && GameManager.Instance.IsCombat); // 타겟 없고 난전상태면 조사

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, ChaseState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // 타겟 발견하면 추격, 난전시작
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasTarget && GameManager.Instance.IsCombat); // 타겟 없고 난전상태면 조사

        // Chase
        AddTransition<ChaseState, AttackState>(() => attackState.IsTargetInAttackRange && GameManager.Instance.IsCombat); // 사정거리 안에 들어오면 공격
        AddTransition<ChaseState, SuspectState>(() => owner.HasTarget && !GameManager.Instance.IsCombat); // 사정거리 안에 들어오면 공격
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
