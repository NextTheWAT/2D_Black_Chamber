using System.Collections;
using UnityEngine;
using Constants;

public class InvestigateState : BaseState
{
    private float investigateTimer = 0f;
    public bool IsInvestigating => investigateTimer < owner.Data.investigateDuration || GameManager.Instance.IsCombat;

    private Coroutine investigateCoroutine;

    public InvestigateState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        BeginInvestigate();
        ConditionalLogger.Log("InvestigateState Enter");
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

        owner.AutoRotate = true;
    }

    private void StopInvestigate()
    {
        if (investigateCoroutine != null)
        {
            owner.StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
        }

        owner.AutoRotate = false;
    }

    private IEnumerator InvestigateLoop()
    {
        // 조사 시작 전 대기
        Vector2 dirToLastKnown = (owner.LastKnownTargetPos - (Vector2)owner.transform.position).normalized;
        owner.LookPoint = (Vector2)owner.transform.position + dirToLastKnown;
        owner.Agent.isStopped = true;
        yield return new WaitForSeconds(owner.Data.investigateStartDelay);
        owner.Agent.isStopped = false;

        // 처음 플레이어 위치로 이동
        owner.MoveTo(owner.LastKnownTargetPos);
        investigateTimer = 0f;

        while (investigateTimer < owner.Data.investigateDuration)
        {
            if (owner.IsArrived) break;
            investigateTimer += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            // 랜덤한 조사 지점으로 이동
            do
            {
                owner.MoveTo(GetRandomInvestigatePoint());
                investigateTimer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // 목적지에 도착할 때까지 대기
            while (!owner.IsArrived)
            {
                investigateTimer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(owner.Data.investigatePauseDuration);
        }
    }

    // 조사할 랜덤 지점 생성
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * owner.Data.investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
