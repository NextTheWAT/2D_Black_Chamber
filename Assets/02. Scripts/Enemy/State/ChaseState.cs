using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChaseState : BaseState
{
    private readonly float initialChaseDelay;
    private readonly float investigateThreshold;

    private Coroutine chaseCoroutine;
    private float chaseTimer = 0f;

    public bool IsChasing => chaseTimer < investigateThreshold;

    public ChaseState(Enemy owner, float initialChaseDelay, float investigateThreshold) : base(owner)
    {
        this.initialChaseDelay = initialChaseDelay;
        this.investigateThreshold = investigateThreshold;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("ChaseState Enter");
        BeginChase();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ChaseState Exit");
        StopChase();
        owner.Agent.isStopped = false;
    }

    private void BeginChase()
    {
        if (chaseCoroutine != null)
            owner.StopCoroutine(chaseCoroutine);
        chaseCoroutine = owner.StartCoroutine(ChaseLoop());
    }

    private void StopChase()
    {
        if (chaseCoroutine != null)
        {
            owner.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
    }

    private IEnumerator ChaseLoop()
    {
        /*
        // 발견 이펙트 생성
        owner.Agent.isStopped = true;
        ConditionalLogger.Log("Find Enemy!");
        float timer = 0f;

        while (timer < initialChaseDelay)
        {
            owner.FindTarget();
            owner.LookAt(owner.LastKnownTargetPos);
            timer += Time.deltaTime;
            yield return null;
        }
        ConditionalLogger.Log("Start Chase!");
        */
        owner.Agent.isStopped = false;
        chaseTimer = 0f;

        while (true)
        {
            owner.FindTarget();

            if (owner.HasTarget)
            {
                chaseTimer = 0;
                owner.MoveTo(owner.Target.position);
            }
            else
            {
                chaseTimer += Time.deltaTime;
            }
            yield return null;
        }
    }

}
