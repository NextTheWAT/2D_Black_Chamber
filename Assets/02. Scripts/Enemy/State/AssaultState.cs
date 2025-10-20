
using UnityEngine;

public class AssaultState : BaseState
{
    public AssaultState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Target = GameManager.Instance.Player;
        ConditionalLogger.Log("AssaultState Enter");
    }

    public override void Update()
    {
        if (owner.HasTarget)
            owner.MoveTo(owner.Target.position);
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AssaultState Exit");
    }
}
