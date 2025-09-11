using System.Collections;
using UnityEngine;
using Constants;

public class PatrolState : BaseState
{
    private int currentPointIndex = 0;
    private readonly float waitTime = 2f;
    private Coroutine patrolCoroutine;

    public PatrolState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolState Enter");
        currentPointIndex = owner.StartPatrolPointIndex;
        BeginPatrol();
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
            owner.ChangeState<ChaseState>();
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
            if (owner.PatrolType == PatrolType.Waypoint)
            {
                if (owner.PatrolPoints.Length == 0) yield break;

                Vector2 destination = owner.PatrolPoints[currentPointIndex].position;
                owner.MoveTo(destination);

                while (owner.Agent.pathPending || owner.Agent.remainingDistance > 0.1f)
                    yield return null;

                yield return new WaitForSeconds(waitTime);
                currentPointIndex = (currentPointIndex + 1) % owner.PatrolPoints.Length;
            }
            else if (owner.PatrolType == PatrolType.Fixed)
            {
                float randomAngle = Random.Range(0f, 360f);

                while (Mathf.Abs(owner.transform.eulerAngles.z - randomAngle) > 1f)
                {
                    owner.RotateTo(randomAngle);
                    yield return null;
                }

                yield return new WaitForSeconds(waitTime);
            }
        }
    }

}
