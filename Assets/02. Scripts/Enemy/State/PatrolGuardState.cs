using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGuardState : BaseState
{
    private float waitTime = 2f;
    private Coroutine patrolCoroutine;
    private float rotateSpeed = 5f;

    public PatrolGuardState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        ConditionalLogger.Log("PatrolGuardState Enter");
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
        ConditionalLogger.Log("PatrolGuardState Exit");
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
            float randomAngle = GetRandomAngle();

            while (Mathf.Abs(owner.transform.eulerAngles.z - randomAngle) > 1f)
            {
                RotateTo(randomAngle);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }
    public void RotateTo(float targetAngle)
    {
        float angle = Mathf.LerpAngle(owner.transform.eulerAngles.z, targetAngle, Time.deltaTime * rotateSpeed);
        owner.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private float GetRandomAngle()
    {
        return Random.Range(0f, 360f);
    }

}
