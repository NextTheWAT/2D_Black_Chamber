public class ReturnState : BaseState
{
    public ReturnState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("ReturnState Enter");
        if (owner.ReturnPoint)
            owner.MoveTo(owner.ReturnPoint.position);
    }

    public override void Update()
    {
        owner.FindTarget();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ReturnState Exit");
    }
}
