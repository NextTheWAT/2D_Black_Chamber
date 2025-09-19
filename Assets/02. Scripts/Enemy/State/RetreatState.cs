using UnityEngine;

public class RetreatState : BaseState
{
    private readonly float retreatHealthRatio = .2f; // �����ϴ� ü�� ���� ex) .2f = 20%
    private readonly float retreatDistance = 5f; // ���� �Ÿ�
    private readonly float returnTime = 5f; // ���� �� ���� �ð�
    private float retreatHealthThreshold; // ���� ü�� �Ӱ谪
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

        // �÷��̾ �þ߿� ���̴��� Ȯ��
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
