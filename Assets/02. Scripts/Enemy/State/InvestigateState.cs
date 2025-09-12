using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class InvestigateState : BaseState
{
    private float waitTime = 1f;
    private Coroutine investigateCoroutine;

    public InvestigateState(Enemy owner) : base(owner) { }
    public override StateType StateType => StateType.Investigate;

    public override void Enter()
    {
        BeginInvestigate();
        ConditionalLogger.Log("InvestigateState Enter");
    }

    public override void Update()
    {
        owner.FindTarget();
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
        // 처음 플레이어 위치로 이동
        owner.MoveTo(owner.LastKnownTargetPos);
        owner.investigateTimer = 0f;

        while (owner.investigateTimer < owner.investigateDuration)
        {
            if (owner.IsArrived) break;
            owner.investigateTimer += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            // 랜덤한 조사 지점으로 이동
            do
            {
                owner.MoveTo(GetRandomInvestigatePoint());
                owner.investigateTimer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // 목적지에 도착할 때까지 대기
            while (!owner.IsArrived)
            {
                owner.investigateTimer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    // 조사할 랜덤 지점 생성
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * owner.investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
