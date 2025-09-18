using System.Collections;
using UnityEngine;
using Constants;

public class PatrolState : BaseState
{
    private readonly PatrolType patrolType; // ���� ����
    private readonly float patrolPauseTime = 2f; // ���� �������� ��� �ð�
    private readonly float fixedPatrolAngle = 180f; // ���� ���� �� ȸ�� ����
    private readonly float originalEulerAngle = 0f;
    private readonly float halfFixedPatrolAngle;

    private int currentPointIndex = 0;
    private Coroutine patrolCoroutine;

    public PatrolState(Enemy owner, PatrolType patrolType, float patrolPauseTime, float fixedPatrolAngle) : base(owner)
    {
        this.patrolType = patrolType;
        this.patrolPauseTime = patrolPauseTime;
        this.fixedPatrolAngle = fixedPatrolAngle;
        originalEulerAngle = owner.transform.eulerAngles.z;
        halfFixedPatrolAngle = fixedPatrolAngle / 2f;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolState Enter");
        currentPointIndex = owner.StartPatrolPointIndex;
        BeginPatrol();
    }

    public override void Update()
    {
        owner.FindSuspiciousTarget();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("PatrolState Exit");
        StopPatrol();
    }

    private void BeginPatrol()
    {
        if (patrolCoroutine != null)
            owner.StopCoroutine(patrolCoroutine);
        patrolCoroutine = owner.StartCoroutine(PatrolLoop());
    }

    private void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            owner.StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }

    private IEnumerator PatrolLoop()
    {
        while (true)
        {
            if (patrolType == PatrolType.Waypoint)
            {
                if (owner.PatrolPoints.Length == 0) yield break;
                if (owner.PatrolPoints[currentPointIndex] == null) yield break;

                Vector2 destination = owner.PatrolPoints[currentPointIndex].position;
                owner.MoveTo(destination);

                while (owner.Agent.pathPending || owner.Agent.remainingDistance > 0.1f)
                    yield return null;

                yield return new WaitForSeconds(patrolPauseTime);
                currentPointIndex = (currentPointIndex + 1) % owner.PatrolPoints.Length;
            }
            else if (patrolType == PatrolType.Fixed)
            {
                // ���� ���� ����
                float randomAngle = Random.Range(-halfFixedPatrolAngle, halfFixedPatrolAngle);
                float targetAngle = originalEulerAngle + randomAngle;
                float currentAngle = owner.transform.eulerAngles.z;

                // ������ 0~360 ������ ����
                targetAngle = Mathf.Repeat(targetAngle, 360f);

                // LookPoint ���
                float rad = (targetAngle + 90) * Mathf.Deg2Rad; // LookPoint ����� ���� 90�� ����
                owner.LookPoint = (Vector2)owner.transform.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                
                // ȸ���� ������ ���
                
                while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 1f)
                {
                    currentAngle = owner.transform.eulerAngles.z;
                    ConditionalLogger.Log($"CurrentAngle: {currentAngle}, TargetAngle: {targetAngle} DeltaAngle {Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle))}");
                    yield return null;
                }
                
                
                yield return new WaitForSeconds(patrolPauseTime);
            }
        }
    }

}
