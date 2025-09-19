using System;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable, Type startType) : base(owner, stateTable, startType)
    {
        FleeState fleeState = GetState<FleeState>();

        // Global
        AddGlobalTransition<FleeState>(() => owner.IsHit, ()=> GameManager.Instance.IsCombat = true); // 피격시 도망 및 전투 모드 진입
        AddGlobalTransition<FleeState>(() => GameManager.Instance.IsCombat && CurrentState.GetType() != typeof(FleeState)); // 전투 모드 진입시 도망

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // 근처에 타겟 있으면 의심

        // Suspect
        AddTransition<SuspectState, PatrolState>(() => !owner.HasTargetInFOV); // 근처에 타겟 없으면 다시 순찰
        AddTransition<SuspectState, FleeState>(() => owner.HasTarget); // 타겟 발견하면 도망

        // Flee
        AddTransition<FleeState, PatrolState>(() => !fleeState.IsFleeing); // 도망 완료시 순찰

        // Return
        AddTransition<ReturnState, FleeState>(() => owner.HasTarget); // 타겟 발견시 도망
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived); // 귀환 완료시 순찰
    }
}
