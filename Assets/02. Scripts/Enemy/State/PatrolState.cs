using System.Collections;
using UnityEngine;
using Constants;

public class PatrolState : BaseState
{
    private readonly PatrolType patrolType; // 순찰 유형
    private readonly float patrolPauseTime = 2f; // 순찰 지점에서 대기 시간
    private readonly float fixedPatrolAngle = 180f; // 고정 순찰 시 회전 각도
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
                float randomAngle = Random.Range(-halfFixedPatrolAngle, halfFixedPatrolAngle);
                float targetAngle = originalEulerAngle + randomAngle;

                while (Mathf.Abs(Mathf.DeltaAngle(owner.transform.eulerAngles.z, targetAngle)) > 1f)
                {
                    owner.RotateTo(targetAngle);
                    yield return null;
                }

                yield return new WaitForSeconds(patrolPauseTime);
            }
        }
    }

}
