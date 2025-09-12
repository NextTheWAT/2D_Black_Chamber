using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : BaseState
{
    private float investigateDuration = 5f;
    private float investigateRange = 2f;
    private float waitTime = 1f;
    private Coroutine investigateCoroutine;

    public InvestigateState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        BeginInvestigate();
        ConditionalLogger.Log("InvestigateState Enter");
    }

    public override void Update()
    {
        owner.FindTarget();

        if (owner.HasTarget)
            owner.ChangeState<ChaseState>();
    }

    public override void Exit()
    {
        StopInvestigate();
        ConditionalLogger.Log("InvestigateState Exit");
    }

    private void BeginInvestigate()
    {
        if (investigateCoroutine != null)
            owner.StopCoroutine(investigateCoroutine);
        investigateCoroutine = owner.StartCoroutine(InvestigateLoop());
    }

    private void StopInvestigate()
    {
        if (investigateCoroutine != null)
        {
            owner.StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
        }
    }

    private IEnumerator InvestigateLoop()
    {
        float timer = 0f;

        // 처음 플레이어 위치로 이동
        owner.MoveTo(owner.LastKnownTargetPos);
        yield return null;
        while (timer < investigateDuration)
        {
            if (owner.Agent.remainingDistance < 0.01f) break;
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < investigateDuration)
        {
            // 랜덤한 조사 지점으로 이동
            do
            {
                GetRandomInvestigatePoint();
                owner.MoveTo(GetRandomInvestigatePoint());
                timer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // 목적지에 도착할 때까지 대기
            while (owner.Agent.remainingDistance > 0.01f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }

        // 조사 시간이 끝나면 복귀 상태로 전환
        owner.ChangeState<ReturnState>();
    }

    // 조사할 랜덤 지점 생성
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
