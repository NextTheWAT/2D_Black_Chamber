
using UnityEngine;

public class AssaultState : BaseState
{
    private LayerMask targetLayer;

    public AssaultState(Enemy owner, LayerMask targetLayer) : base(owner)
    {
        this.targetLayer = targetLayer;
    }

    public override void Enter()
    {
        owner.Target = GameManager.Instance.player;
        ConditionalLogger.Log("AssaultState Enter");
    }

    public override void Update()
    {
        if (owner.HasTarget)
        {
            owner.MoveTo(owner.Target.position);
            GameManager.Instance.RefreshCombatTimer();
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AssaultState Exit");
    }
}
