using UnityEngine;

public class RetreatState : BaseState
{
    private readonly float retreatHealthRatio = .2f; // 후퇴하는 체력 비율 ex) .2f = 20%
    private readonly float retreatDistance = 5f; // 후퇴 거리
    private readonly float returnTime = 5f; // 후퇴 후 복귀 시간
    private float retreatHealthThreshold; // 후퇴 체력 임계값
    private float retreatTimer = 0f;

    public bool ShouldRetreat => owner.Health.CurrentHealth <= retreatHealthThreshold;
    public bool IsRetreating => retreatTimer < returnTime;

    public RetreatState(Enemy owner, float retreatHealthRatio, float retreatDistance, float returnTime) : base(owner)
    {
        this.retreatHealthRatio = retreatHealthRatio;
        this.retreatDistance = retreatDistance;
        this.returnTime = returnTime;
        retreatHealthThreshold = owner.Health.MaxHealth * retreatHealthRatio;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("RetreatState Enter");
        owner.Target = GameManager.Instance.player;
        owner.MoveTo(CalculateRetreatPoint());
        retreatTimer = 0f;
    }

    public override void Update()
    {
        if (owner.IsHit)
            retreatTimer = 0f;
        else
            retreatTimer += Time.deltaTime;

        // 플레이어가 시야에 보이는지 확인
        if (owner.IsTargetInSight)
        {
            owner.LookPoint = owner.Target.position;
            if (owner.HasTargetInFOV)
                owner.Attack();
        }
        else
        {
            if(!owner.IsArrived)
                owner.LookPoint = (Vector2)owner.transform.position - owner.MoveDirection;
        }

    }

    public override void Exit()
    {
        ConditionalLogger.Log("RetreatState Exit");
        retreatHealthThreshold = owner.Health.CurrentHealth * retreatHealthRatio;
    }

    private Vector2 CalculateRetreatPoint()
    {
        Vector2 directionAwayFromPlayer = (owner.transform.position - owner.Target.position).normalized;
        return (Vector2)owner.transform.position + directionAwayFromPlayer * retreatDistance;
    }
}
