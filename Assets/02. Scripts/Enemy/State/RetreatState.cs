using UnityEngine;

public class RetreatState : BaseState
{
    private readonly float retreatHealthRatio = .2f; // 후퇴하는 체력 비율 ex) .2f = 20%
    private readonly float retreatDistance = 5f; // 후퇴 거리
    private readonly float retreatHealthThreshold; // 후퇴 체력 임계값

    public bool ShouldRetreat => owner.Health.CurrentHealth <= retreatHealthThreshold;

    public RetreatState(Enemy owner, float retreatHealthRatio, float retreatDistance) : base(owner)
    {
        this.retreatHealthRatio = retreatHealthRatio;
        this.retreatDistance = retreatDistance;
        retreatHealthThreshold = owner.Health.MaxHealth * retreatHealthRatio;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("RetreatState Enter");
        owner.Target = GameManager.Instance.player;
        owner.MoveTo(CalculateRetreatPoint());
    }

    public override void Update()
    {
        // 플레이어가 시야에 보이는지 확인
        if (owner.IsTargetInSight)
        {
            owner.LookPoint = owner.Target.position;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("RetreatState Exit");
    }

    private Vector2 CalculateRetreatPoint()
    {
        Vector2 directionAwayFromPlayer = (owner.transform.position - owner.Target.position).normalized;
        return (Vector2)owner.transform.position + directionAwayFromPlayer * retreatDistance;
    }
}
