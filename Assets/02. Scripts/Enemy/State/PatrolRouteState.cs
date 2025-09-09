using System.Collections;
using UnityEngine;

public class PatrolRouteState : BaseState
{
    // protected Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private float waitTime = 2f;
    private Coroutine patrolCoroutine;

    public PatrolRouteState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolState Enter");
        owner.Agent.isStopped = false;
        owner.StopDistance = 0f;
        StartPatrol();
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
        {
            owner.ChangeState<ChaseState>();
            return;
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("PatrolState Exit");
        StopPatrol();
        owner.Agent.isStopped = true;
    }

    private void StartPatrol()
    {
        if (patrolCoroutine != null)
            owner.StopCoroutine(patrolCoroutine);
        patrolCoroutine = owner.StartCoroutine(Patrol());
    }

    private void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            owner.StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (owner.patrolPoints.Length == 0) yield break;

            Vector2 destination = owner.patrolPoints[currentPointIndex].position;
            owner.MoveTo(destination);

            while (owner.Agent.pathPending || owner.Agent.remainingDistance > 0.1f)
                yield return null;

            yield return new WaitForSeconds(waitTime);
            currentPointIndex = (currentPointIndex + 1) % owner.patrolPoints.Length;
        }
    }

}
