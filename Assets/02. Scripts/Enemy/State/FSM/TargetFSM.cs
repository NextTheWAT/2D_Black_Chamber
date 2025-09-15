using Constants;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        FleeState fleeState = GetState<FleeState>();


        // Global
        AddGlobalTransition<FleeState>(() => owner.IsHit);

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ǽ�

        // Suspect
        AddTransition<SuspectState, PatrolState>(() => !owner.HasSuspiciousTarget); // ��ó�� Ÿ�� ������ �ٽ� ����
        AddTransition<SuspectState, FleeState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� ����

        // Flee
        AddTransition<FleeState, PatrolState>(() => !fleeState.IsFleeing);

        // Return
        AddTransition<ReturnState, FleeState>(() => owner.HasTarget);
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);
    }
}
