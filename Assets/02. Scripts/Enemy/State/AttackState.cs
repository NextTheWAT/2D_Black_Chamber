using UnityEngine;

public class AttackState : BaseState
{
    private readonly float maxAttackRange = 6f; // ���� ����
    private readonly float desiredAttackDistance = 3f; // ������ ���ϴ� ���� �Ÿ�

    public AttackState(Enemy owner, float maxAttackRange, float desiredAttackDistance) : base(owner)
    {
        this.maxAttackRange = maxAttackRange;
        this.desiredAttackDistance = desiredAttackDistance;
    }

    public bool IsTargetInAttackRange
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= maxAttackRange;
        }
    }


    public bool IsTargetInDesiredAttackDistance
    {
        get
        {
            if (!owner.HasTarget) return false;
            float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
            return distToTarget <= desiredAttackDistance;
        }
    }

    public override void Enter()
        => ConditionalLogger.Log("AttackState Enter");

    public override void Update()
    {

        if (owner.HasTargetInFOV)
        {
            owner.LookPoint = owner.Target.position;
            owner.Agent.isStopped = false;

            // Ÿ���� ���� ���� ���� �ִ��� Ȯ��
            if (IsTargetInAttackRange)
            {
                owner.Attack();
                owner.AnimationController.SetActiveShoot(true);
                if (!IsTargetInDesiredAttackDistance)
                    owner.MoveTo(owner.Target.position);
                else
                    owner.Agent.isStopped = true;
            }
            else
            {
                owner.MoveTo(owner.Target.position);
                owner.AnimationController.SetActiveShoot(false);
            }
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("AttackState Exit");
        owner.AnimationController.SetActiveShoot(false);
        owner.Agent.isStopped = false;
    }
}
