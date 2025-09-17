public class DeathState : BaseState
{
    public DeathState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("DeathState Enter");
        owner.Die();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("DeathState Exit");
    }
}
