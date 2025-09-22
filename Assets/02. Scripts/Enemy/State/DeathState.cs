using UnityEngine;

public class DeathState : BaseState
{
    private float deathSignalRadius = 5f;
    private LayerMask deathSignalMask;

    public DeathState(Enemy owner, float deathSignalRadius, LayerMask deathSignalMask) : base(owner)
    {
        this.deathSignalRadius = deathSignalRadius;
        this.deathSignalMask = deathSignalMask;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("DeathState Enter");
        SendDeathSignal();
        owner.Die();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("DeathState Exit");
    }

    private void SendDeathSignal()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position, deathSignalRadius, deathSignalMask);
        foreach (var col in colliders)
        {
            var enemy = col.GetComponent<Enemy>();
            if (enemy)
                enemy.NearbyDeathTriggered = true;
        }
    }
}
