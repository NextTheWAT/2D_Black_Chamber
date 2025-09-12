using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    private List<Transition> transitions = new();

    public IState CurrentState => currentState;
    private Dictionary<StateType, IState> states = new();

    protected Enemy owner;

    public StateMachine(Enemy owner, StateTable stateTable)
    {
        this.owner = owner;

        foreach (var stateType in stateTable.stateTypes)
        {
            var state = StateFactory.CreateState(stateType, owner);
            if (state != null)
                AddState(stateType, state);
            else
                ConditionalLogger.LogWarning($"StateFactory에서 {stateType} 상태를 생성하지 못했습니다.");
        }

        ChangeState(stateTable.stateTypes[stateTable.startStateIndex]);
    }

    public void AddTransition(StateType from, StateType to, Func<bool> condition)
    {
        if (!states.ContainsKey(from) || !states.ContainsKey(to))
        {
            ConditionalLogger.LogWarning($"Transition 추가 실패: {from} 또는 {to} 상태가 존재하지 않습니다.");
            return;
        }

        transitions.Add(new Transition(states[from], states[to], condition));
    }

    public void AddState(StateType stateType, IState state)
    {
        if (states.ContainsKey(stateType))
        {
            ConditionalLogger.LogWarning($"이미 {state} 상태가 존재합니다.");
            return;
        }

        states[stateType] = state;
    }

    public void ChangeState(StateType stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            ConditionalLogger.LogWarning($"해당 {stateType}가 존재하지 않습니다.");
            return;
        }

        IState state = states[stateType];
        if (currentState != null && currentState == state) return;

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void UpdateState()
    {
        if (currentState == null) return;

        // 현재 상태 업데이트
        currentState.Update();

        // Transition 체크
        foreach (var t in transitions)
        {
            if (t.FromState != currentState) continue;

            if (t.Condition())
            {
                ChangeState(t.ToState.StateType);
                break; // 한 번에 하나만 전환
            }
        }
    }
}
