using System;
using System.Collections.Generic;
public class StateMachine
{
    private readonly Dictionary<Type, IState> states = new();
    private readonly List<Transition> transitions = new(); // 특정 상태에서 적용되는 전환
    private readonly List<Transition> globalTransitions = new(); // 모든 상태에서 적용되는 전환
    private IState currentState;
    public IState CurrentState => currentState;
    private readonly IState startState;

    protected Enemy owner;

    public StateMachine(Enemy owner, StateTable stateTable, Type startType)
    {
        // 상태 초기화
        this.owner = owner;
        states = StateFactory.CreateStates(owner, stateTable);
        if (states.Count == 0)
            ConditionalLogger.LogWarning("StateMachine에 상태가 하나도 없습니다.");

        // 초기 상태 설정
        if (states.ContainsKey(startType))
            startState = states[startType];
        else
        {
            ConditionalLogger.LogWarning($"StateMachine에 {startType} 상태가 존재하지 않습니다.");
        }
    }


    public T GetState<T>() where T : class, IState
    {

        if (!states.ContainsKey(typeof(T)))
        {
            ConditionalLogger.LogWarning($"StateMachine에 {typeof(T)} 상태가 존재하지 않습니다.");
            return null;
        }
        return states[typeof(T)] as T;
    }

    public void Start()
    {
        ConditionalLogger.Log("StateMachine Start");
        ChangeState(startState);
    }

    public void Stop()
    {
        currentState?.Exit();
        currentState = null;
    }

    public void ChangeState(IState state)
    {
        if (currentState != null && currentState == state) return;
        if(state == null) return;
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
