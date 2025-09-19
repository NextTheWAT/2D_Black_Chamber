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
        // 상태 초기화
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

    public void ChangeState(IState state)
    {
        if (currentState != null && currentState == state) return;
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void AddTransition<TFrom, TTo>(Func<bool> condition, Action callback = null) where TFrom : IState where TTo : IState
    {
        Type from = typeof(TFrom);
        Type to = typeof(TTo);

        if (!states.ContainsKey(from))
        {
            ConditionalLogger.LogWarning($"StateMachine에 {from} 상태가 존재하지 않습니다.");
            return;
        }
        if (!states.ContainsKey(to))
        {
            ConditionalLogger.LogWarning($"StateMachine에 {to} 상태가 존재하지 않습니다.");
            return;
        }

        transitions.Add(new Transition(states[from], states[to], condition, callback));
    }

    public void AddGlobalTransition<TTo>(Func<bool> condition, Action callback = null) where TTo : IState
    {
        Type to = typeof(TTo);

        if (!states.ContainsKey(to))
        {
            ConditionalLogger.LogWarning($"StateMachine에 {to} 상태가 존재하지 않습니다.");
            return;
        }

        globalTransitions.Add(new Transition(null, states[to], condition, callback));
    }

    public void UpdateState()
    {
        if (currentState == null) return;

        // 현재 상태 업데이트
        currentState.Update();

        // GlobalTransition 체크
        foreach (var t in globalTransitions)
        {
            if (t.ToState == currentState) continue;

            if (t.Condition())
            {
                t.Callback?.Invoke();
                ChangeState(t.ToState);
                break; // 한 번에 하나만 전환
            }
        }

        // Transition 체크
        foreach (var t in transitions)
        {
            if (t.FromState != currentState) continue;
            if (t.ToState == currentState) continue;

            if (t.Condition())
            {
                t.Callback?.Invoke();
                ChangeState(t.ToState);
                break; // 한 번에 하나만 전환
            }
        }
    }
}
