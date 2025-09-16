
using UnityEngine;

public class AssaultState : BaseState
{
    private LayerMask targetLayer;

    public AssaultState(Enemy owner, LayerMask targetLayer) : base(owner)
    {
        this.targetLayer = targetLayer;
    }

    public bool CanAttack
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, owner.transform.up, 20f, targetLayer);
            return hit.collider != null && hit.collider.CompareTag("Player");
        }
    }

    public override void Enter()
    {
        owner.Target = GameManager.Instance.player;
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
