using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    private Dictionary<Type, IState> states = new();
    private List<Transition> transitions = new(); // Ư�� ���¿��� ����Ǵ� ��ȯ
    private List<Transition> globalTransitions = new(); // ��� ���¿��� ����Ǵ� ��ȯ

    public IState CurrentState => currentState;

    protected Enemy owner;

    public StateMachine(Enemy owner, StateTable stateTable)
    {
        this.owner = owner;
        states = StateFactory.CreateStates(owner, stateTable);
        if (states.Count == 0)
            ConditionalLogger.LogWarning("StateMachine�� ���°� �ϳ��� �����ϴ�.");

        // �ʱ� ���� ����
        IState startState = states[stateTable.StartStateType];
        ChangeState(startState);
    }

    public T GetState<T>() where T : class, IState
        => states[typeof(T)] as T;

    public void AddState(IState state)
    {
        Type type = state.GetType();

        if (states.ContainsKey(type))
        {
            ConditionalLogger.LogWarning($"�̹� {state} ���°� �����մϴ�.");
            return;
        }

        states[type] = state;
    }

    public void ChangeState(IState state)
    {
        if (currentState != null && currentState == state) return;
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void AddTransition<TFrom, TTo>(Func<bool> condition) where TFrom : IState where TTo : IState
    {
        Type from = typeof(TFrom);
        Type to = typeof(TTo);
        transitions.Add(new Transition(states[from], states[to], condition));
    }


    public void AddGlobalTransition<TTo>(Func<bool> condition) where TTo : IState
    {
        Type to = typeof(TTo);
        globalTransitions.Add(new Transition(null, states[to], condition));
    }

    public void UpdateState()
    {
        if (currentState == null) return;

        // ���� ���� ������Ʈ
        currentState.Update();

        // GlobalTransition üũ
        foreach (var t in globalTransitions)
        {
            if (t.Condition())
            {
                ChangeState(t.ToState);
                break; // �� ���� �ϳ��� ��ȯ
            }
        }

        // Transition üũ
        foreach (var t in transitions)
        {
            if (t.FromState != currentState) continue;

            if (t.Condition())
            {
                ChangeState(t.ToState);
                break; // �� ���� �ϳ��� ��ȯ
            }
        }
    }
}
