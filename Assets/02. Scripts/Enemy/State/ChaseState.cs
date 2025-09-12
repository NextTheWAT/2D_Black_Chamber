using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ChaseState : BaseState
{
    private Coroutine chaseCoroutine;

    public ChaseState(Enemy owner) : base(owner) { }
    public override StateType StateType => StateType.Chase;

    public override void Enter()
    {
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

        while (chaseTimer < owner.InitialChaseDelay)
        {
            owner.FindTarget();
            owner.LookAt(owner.LastKnownTargetPos);
            chaseTimer += Time.deltaTime;
            yield return null;
        }

        ConditionalLogger.Log("Start Chase!");
        owner.Agent.isStopped = false;
        owner.chaseToInvestigateTimer = 0f;

        while (true)
        {
            owner.FindTarget();

            if (owner.HasTarget)
            {
                owner.chaseToInvestigateTimer = 0;
                owner.MoveTo(owner.Target.position);
            }
            else
            {
                owner.chaseToInvestigateTimer += Time.deltaTime;
            }
            yield return null;
        }
    }

}
