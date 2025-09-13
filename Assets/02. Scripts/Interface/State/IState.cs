using Constants;

public interface IState
{
    void Enter();
    void Exit();
    void Update();
    public StateType StateType { get; }
}
