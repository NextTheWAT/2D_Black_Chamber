using System;

public class TargetFSM : StateMachine
{
    public TargetFSM(Enemy owner, StateTable stateTable, Type startType) : base(owner, stateTable, startType)
    {
        FleeState fleeState = GetState<FleeState>();

        // Global
        AddGlobalTransition<FleeState>(() => owner.IsHit, ()=> GameManager.Instance.IsCombat = true); // �ǰݽ� ���� �� ���� ��� ����
        AddGlobalTransition<FleeState>(() => GameManager.Instance.IsCombat && CurrentState.GetType() != typeof(FleeState)); // ���� ��� ���Խ� ����

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ǽ�

        // Suspect
        AddTransition<SuspectState, PatrolState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ٽ� ����
        AddTransition<SuspectState, FleeState>(() => owner.HasTarget); // Ÿ�� �߰��ϸ� ����

        // Flee
        AddTransition<FleeState, PatrolState>(() => !fleeState.IsFleeing); // ���� �Ϸ�� ����

        // Return
        AddTransition<ReturnState, FleeState>(() => owner.HasTarget); // Ÿ�� �߽߰� ����
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived); // ��ȯ �Ϸ�� ����
    }
}
