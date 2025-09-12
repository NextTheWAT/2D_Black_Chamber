using Constants;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        // Global
        AddGlobalTransition(StateType.Flee, () => owner.IsHit);

        // Patrol
        AddTransition(StateType.Patrol, StateType.Flee, () => owner.HasTarget);

        // Flee
        AddTransition(StateType.Flee, StateType.Patrol, () => owner.IsFleeStop);

        // Return
        AddTransition(StateType.Return, StateType.Flee, () => owner.HasTarget);
        AddTransition(StateType.Return, StateType.Patrol, () => owner.IsArrived);
    }
}
