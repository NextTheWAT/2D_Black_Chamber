public class SoliderFSM : StateMachine
{
    public SoliderFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        InvestigateState investigateState = GetState<InvestigateState>();
        AssaultState assaultState = GetState<AssaultState>();
        AttackState attackState = GetState<AttackState>();
        RetreatState retreatState = GetState<RetreatState>();

        // 공용
        AddGlobalTransition<DeathState>(() => owner.IsDead); // 사망

        // 잠입

        // Global
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // 맞았을 때 의심상태면 추격 및 난전 시작
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState)); // 맞았을 때 공격상태가 아니면 엄폐

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // 근처에 타겟 있으면 의심
        AddTransition<PatrolState, CoverState>(() => GameManager.Instance.IsCombat); // 난전 시작되면 엄폐

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasSuspiciousTarget); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, CoverState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // 타겟 발견하면 추격, 난전시작
        AddTransition<SuspectState, CoverState>(() => GameManager.Instance.IsCombat); // 난전 시작되면 엄폐

        // Investigate
        AddTransition<InvestigateState, SuspectState>(() => owner.HasSuspiciousTarget); // 타겟 발견하면 추격
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // 조사 시간 끝나면 복귀

        // Return
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

        // 난전
        AddGlobalTransition<RetreatState>(() => owner.NearbyDeathTriggered); // 근처 아군 사망시 후퇴
        AddGlobalTransition<RetreatState>(() => retreatState.ShouldRetreat); // 피격 및 체력 낮으면 후퇴

        // Cover
        AddTransition<CoverState, AttackState>(() => attackState.IsTargetInAttackRange && owner.IsTargetInSight); // 사정거리 안에 들어오면 공격

        // Assault
        AddTransition<AssaultState, AttackState>(() => owner.IsTargetInSight); // 공격 가능하면 공격

        // Attack
        AddTransition<AttackState, CoverState>(() => !attackState.IsTargetInAttackRange || !owner.IsTargetInSight); // 사정거리 벗어나면 추격

    }
}
