using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private Coroutine chaseCoroutine;

    public ChaseState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        if (owner.Target == null)
        {
            owner.ChangeState<PatrolState>();
            return;
        }

        ConditionalLogger.Log("ChaseState Enter");
        BeginChase();
    }

    public override void Exit()
    {
        ConditionalLogger.Log("ChaseState Exit");
        StopChase();
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
        // 발견 이펙트 생성
        owner.Agent.isStopped = true;
        ConditionalLogger.Log("Find Enemy!");
        float chaseTimer = 0f;
        Vector2 lastTargetPos = owner.Target.position;
        
        while (chaseTimer < owner.InitialChaseDelay)
        {
            owner.FindTarget();

            if (owner.HasTarget)
                lastTargetPos = owner.Target.position;

            owner.MoveTo(lastTargetPos);
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        ConditionalLogger.Log("Start Chase!");
        owner.Agent.isStopped = false;
        float investigateTimer = 0f;

        while (true)
        {
            owner.FindTarget();

            if (owner.HasTarget)
            {
                investigateTimer = 0;
                float distToTarget = Vector2.Distance(owner.transform.position, owner.Target.position);
                if (distToTarget <= owner.AttackRange)
                {
                    owner.ChangeState<AttackState>();
                    yield break;
                }
                else
                {
                    owner.MoveTo(owner.Target.position);
                }
            }
            else
            {
                investigateTimer += Time.deltaTime;

                if (investigateTimer >= owner.InvestigateThreshold)
                {
                    owner.ChangeState<InvestigateState>();
                    yield break;
                }
            }
            yield return null;
        }
    }

}
