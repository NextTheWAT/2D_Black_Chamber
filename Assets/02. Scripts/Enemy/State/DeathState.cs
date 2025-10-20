using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(Enemy owner) : base(owner) { }

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position, GameManager.Instance.enemyLayerMask);
        foreach (var col in colliders)
        {
            var enemy = col.GetComponent<Enemy>();
            if (enemy)
                enemy.NearbyDeathTriggered = true;
        }
    }
}
