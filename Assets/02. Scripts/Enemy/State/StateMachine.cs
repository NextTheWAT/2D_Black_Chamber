using Constants;
using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    private List<Transition> transitions = new(); // Ư�� ���¿��� ����Ǵ� ��ȯ
    private List<Transition> globalTransitions = new(); // ��� ���¿��� ����Ǵ� ��ȯ

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
                ConditionalLogger.LogWarning($"StateFactory���� {stateType} ���¸� �������� ���߽��ϴ�.");
        }

        ChangeState(stateTable.stateTypes[stateTable.startStateIndex]);
    }

    public void AddTransition(StateType from, StateType to, Func<bool> condition)
    {
        if (!states.ContainsKey(from) || !states.ContainsKey(to))
        {
            ConditionalLogger.LogWarning($"Transition �߰� ����: {from} �Ǵ� {to} ���°� �������� �ʽ��ϴ�.");
            return;
        }

        transitions.Add(new Transition(states[from], states[to], condition));
    }


    public void AddGlobalTransition(StateType to, Func<bool> condition)
    {
        if (!states.ContainsKey(to))
        {
            ConditionalLogger.LogWarning($"GlobalTransition �߰� ����: {to} ���°� �������� �ʽ��ϴ�.");
            return;
        }

        ConditionalLogger.Log($"GlobalTransition �߰�: -> {to}");
        globalTransitions.Add(new Transition(null, states[to], condition));
    }

    public void AddState(StateType stateType, IState state)
    {
        if (states.ContainsKey(stateType))
        {
            ConditionalLogger.LogWarning($"�̹� {state} ���°� �����մϴ�.");
            return;
        }

        states[stateType] = state;
    }

    public void ChangeState(StateType stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            ConditionalLogger.LogWarning($"�ش� {stateType}�� �������� �ʽ��ϴ�.");
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

        // ���� ���� ������Ʈ
        currentState.Update();

        // GlobalTransition üũ
        foreach (var t in globalTransitions)
        {
            if (t.Condition())
            {
                ConditionalLogger.Log($"Global Transition: {currentState.StateType} -> {t.ToState.StateType}");
                ChangeState(t.ToState.StateType);
                break; // �� ���� �ϳ��� ��ȯ
            }
        }

        // Transition üũ
        foreach (var t in transitions)
        {
            if (t.FromState != currentState) continue;

            if (t.Condition())
            {
                ChangeState(t.ToState.StateType);
                break; // �� ���� �ϳ��� ��ȯ
            }
        }
    }
}
