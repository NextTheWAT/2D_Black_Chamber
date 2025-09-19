public class SoliderFSM : StateMachine
{
    public SoliderFSM(Enemy owner, StateTable stateTable) : base(owner, stateTable)
    {
        InvestigateState investigateState = GetState<InvestigateState>();
        AssaultState assaultState = GetState<AssaultState>();
        AttackState attackState = GetState<AttackState>();
        RetreatState retreatState = GetState<RetreatState>();

        // ����
        AddGlobalTransition<DeathState>(() => owner.IsDead); // ���

        // ����

        // Global
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() == typeof(SuspectState), () => GameManager.Instance.IsCombat = true); // �¾��� �� �ǽɻ��¸� �߰� �� ���� ����
        AddGlobalTransition<CoverState>(() => owner.IsHit && CurrentState.GetType() != typeof(AttackState)); // �¾��� �� ���ݻ��°� �ƴϸ� ����

        // Patrol
        AddTransition<PatrolState, SuspectState>(() => owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ �ǽ�
        AddTransition<PatrolState, CoverState>(() => GameManager.Instance.IsCombat); // ���� ���۵Ǹ� ����

        // Suspect
        AddTransition<SuspectState, InvestigateState>(() => !owner.HasTargetInFOV); // ��ó�� Ÿ�� ������ ����
        AddTransition<SuspectState, CoverState>(() => owner.HasTarget, () => GameManager.Instance.IsCombat = true); // Ÿ�� �߰��ϸ� �߰�, ��������
        AddTransition<SuspectState, CoverState>(() => GameManager.Instance.IsCombat); // ���� ���۵Ǹ� ����

        // Investigate
        AddTransition<InvestigateState, SuspectState>(() => owner.HasTargetInFOV); // Ÿ�� �߰��ϸ� �ǽ�
        AddTransition<InvestigateState, ReturnState>(() => !investigateState.IsInvestigating); // ���� �ð� ������ ����

        // Return
        AddTransition<ReturnState, PatrolState>(() => owner.IsArrived);

        // ����
        AddGlobalTransition<RetreatState>(() => owner.NearbyDeathTriggered); // ��ó �Ʊ� ����� ����
        AddGlobalTransition<RetreatState>(() => retreatState.ShouldRetreat); // �ǰ� �� ü�� ������ ����

        // Cover
        AddTransition<CoverState, AttackState>(() => owner.HasTargetInFOV); // �����Ÿ� �ȿ� ������ ����

        // Assault
        AddTransition<AssaultState, AttackState>(() => owner.HasTargetInFOV); // ���� �����ϸ� ����

        // Attack
        AddTransition<AttackState, CoverState>(() => !owner.HasTargetInFOV); // �����Ÿ� ����� �߰�

    }
}
