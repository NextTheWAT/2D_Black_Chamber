using System.Collections;
using UnityEngine;
using Constants;

public class PatrolState : BaseState
{
    private readonly PatrolType patrolType;
    private readonly float patrolPauseTime = 2f;
    private int currentPointIndex = 0;
    private Coroutine patrolCoroutine;

    public PatrolState(Enemy owner, PatrolType patrolType, float patrolPauseTime) : base(owner)
    {
        this.patrolType = patrolType;
        this.patrolPauseTime = patrolPauseTime;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolState Enter");
        currentPointIndex = owner.StartPatrolPointIndex;
        BeginPatrol();
    }

    public override void Update()
    {
        owner.FindTarget();
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
                float randomAngle = Random.Range(0f, 360f);

                while (Mathf.Abs(owner.transform.eulerAngles.z - randomAngle) > 1f)
                {
                    owner.RotateTo(randomAngle);
                    yield return null;
                }

                yield return new WaitForSeconds(patrolPauseTime);
            }
        }
    }

}
