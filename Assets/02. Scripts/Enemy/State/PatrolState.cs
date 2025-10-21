using System.Collections;
using UnityEngine;
using Constants;

public class PatrolState : BaseState
{
    private readonly float originalEulerAngle = 0f;
    private readonly float halfFixedPatrolAngle;

    private int currentPointIndex = 0;
    private Coroutine patrolCoroutine;

    public int NextPointIndex => (currentPointIndex + 1) % owner.PatrolPoints.Length;

    public PatrolType PatrolType
    {
        get
        {
            if(owner.PatrolPoints == null || owner.PatrolPoints.Length <= 1)
                return PatrolType.Fixed;
            else
                return PatrolType.Waypoint;
        }
    }

    public PatrolState(Enemy owner) : base(owner)
    {
        originalEulerAngle = owner.transform.eulerAngles.z;
        halfFixedPatrolAngle = owner.Data.patrolFixedAngle / 2f;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolState Enter");
        currentPointIndex = owner.StartPatrolPointIndex;
        BeginPatrol();
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
            if (PatrolType == PatrolType.Waypoint)
            {
                if (owner.PatrolPoints.Length == 0) yield break;
                if (owner.PatrolPoints[currentPointIndex] == null) yield break;

                Vector2 destination = owner.PatrolPoints[currentPointIndex].position;
                Vector2 dir = (destination - (Vector2)owner.transform.position).normalized;
                owner.MoveTo(destination);
                owner.LookPoint = destination + dir;

                while (!owner.IsArrived)
                    yield return null;

                yield return new WaitForSeconds(owner.Data.patrolPauseTime);
                owner.LookPoint = owner.PatrolPoints[NextPointIndex].position;

                // 회전될 때까지 대기
                while (owner.CurrentLookAngleDelta > 1f)
                    yield return null;

                currentPointIndex = NextPointIndex;
            }
            else if (PatrolType == PatrolType.Fixed)
            {
                // 랜덤 각도 결정
                float randomAngle = Random.Range(-halfFixedPatrolAngle, halfFixedPatrolAngle);
                float targetAngle = originalEulerAngle + randomAngle;
                float currentAngle = owner.transform.eulerAngles.z;

                // 각도를 0~360 범위로 정리
                targetAngle = Mathf.Repeat(targetAngle, 360f);

                // LookPoint 계산
                float rad = (targetAngle + 90) * Mathf.Deg2Rad; // LookPoint 계산을 위해 90도 보정
                owner.LookPoint = (Vector2)owner.transform.position + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                
                // 회전될 때까지 대기
                while (owner.CurrentLookAngleDelta > 1f)
                    yield return null;

                yield return new WaitForSeconds(owner.Data.patrolPauseTime);
            }
        }
    }

}
