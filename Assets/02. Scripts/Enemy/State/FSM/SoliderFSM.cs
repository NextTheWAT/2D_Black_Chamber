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
        // 잠입

        // Global
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // 맞았을 때 의심상태면 추격 및 난전 시작
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState)); // 맞았을 때 공격상태가 아니면 추격

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // 근처에 타겟 있으면 의심
        AddTransition<PatrolState, ChaseState>(() => owner.HasTarget); // 타겟 발견하면 공격
        AddTransition<PatrolState, CoverState>(() => GameManager.Instance.IsCombat); // 난전 시작되면 엄폐


        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, CoverState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // 타겟 발견하면 추격, 난전시작
        AddTransition<SuspectState, CoverState>(() => GameManager.Instance.IsCombat); // 난전 시작되면 엄폐

        // Investigate
        AddTransition<InvestigateState, SuspectState>(() => owner.HasSuspiciousTarget); // 타겟 발견하면 추격
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // 조사 시간 끝나면 복귀

        // Return
        AddTransition<ReturnState, ChaseState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

        // 난전

        // Chase
        AddTransition<ChaseState, CoverState>(() => chaseState.IsTargetInChaseRange); // 사정거리 안에 들어오면 공격

        // Cover
        // AddTransition<CoverState, ChaseState>(() => !chaseState.IsTargetInChaseRange); // 사정거리 벗어나면 추격
        AddTransition<CoverState, AttackState>(() => attackState.IsTargetInAttackRange && owner.IsTargetInSight); // 사정거리 안에 들어오면 공격

        // Assault
        AddTransition<AssaultState, ChaseState>(() => !chaseState.IsTargetInChaseRange); // 사정거리 벗어나면 추격
        AddTransition<AssaultState, AttackState>(() => owner.IsTargetInSight); // 공격 가능하면 공격

        // Attack
        AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange || !owner.IsTargetInSight); // 사정거리 벗어나면 추격

    }
}
