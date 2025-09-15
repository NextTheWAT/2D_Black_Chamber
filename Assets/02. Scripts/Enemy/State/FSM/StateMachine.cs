using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    private Dictionary<Type, IState> states = new();
    private List<Transition> transitions = new(); // 특정 상태에서 적용되는 전환
    private List<Transition> globalTransitions = new(); // 모든 상태에서 적용되는 전환

    public IState CurrentState => currentState;

    protected Enemy owner;

    public StateMachine(Enemy owner, StateTable stateTable)
    {
        this.owner = owner;
        states = StateFactory.CreateStates(owner, stateTable);
        if (states.Count == 0)
            ConditionalLogger.LogWarning("StateMachine에 상태가 하나도 없습니다.");

        // 초기 상태 설정
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
            ConditionalLogger.LogWarning($"이미 {state} 상태가 존재합니다.");
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

        // 현재 상태 업데이트
        currentState.Update();

        // GlobalTransition 체크
        foreach (var t in globalTransitions)
        {
            if (t.Condition())
            {
                ChangeState(t.ToState);
                break; // 한 번에 하나만 전환
            }
        }

        // Transition 체크
        foreach (var t in transitions)
        {
            if (t.FromState != currentState) continue;

            if (t.Condition())
            {
                ChangeState(t.ToState);
                break; // 한 번에 하나만 전환
            }
        }
    }
}
