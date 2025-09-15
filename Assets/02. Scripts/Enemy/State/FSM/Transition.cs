using System;

public class Transition
{
    public IState FromState { get; }
    public IState ToState { get; }
    public Func<bool> Condition { get; }
    public Action Callback { get; }

    public Transition(IState from, IState to, Func<bool> condition, Action callback = null)
    {
        FromState = from;
        ToState = to;
        Condition = condition;
        Callback = callback;
    }
}