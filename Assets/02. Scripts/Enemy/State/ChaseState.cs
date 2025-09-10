using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private float initialChaseDelay = 2f; // 처음 타겟 발견했을 때 정지 시간
    private float investigateThreshold = 3f; // 몇 초 이상 못 보면 조사 상태로 전환
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
        
        while (chaseTimer < initialChaseDelay)
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
                owner.MoveTo(owner.Target.position);
            }
            else
            {
                investigateTimer += Time.deltaTime;

                if (investigateTimer >= investigateThreshold)
                {
                    owner.ChangeState<InvestigateState>();
                    yield break;
                }
            }
            yield return null;
        }
    }

}
