using System.Collections;
using UnityEngine;
using Constants;

public class InvestigateState : BaseState
{
    private readonly float startDelay = 3f; // 조사 시작 전 대기 시간
    private readonly float investigateDuration = 5f; // 조사 상태 지속 시간
    private readonly float investigateRange = 2f; // 조사 중 무작위로 이동하는 범위
    private readonly float pauseDuration = 1f; // 조사 중 멈추는 시간
    private float investigateTimer = 0f;
    public bool IsInvestigating => investigateTimer < investigateDuration || GameManager.Instance.IsCombat;

    private Coroutine investigateCoroutine;

    public InvestigateState(Enemy owner, float startDelay, float investigateDuration, float investigateRange, float pauseDuration) : base(owner)
    {
        this.startDelay = startDelay;
        this.investigateDuration = investigateDuration;
        this.investigateRange = investigateRange;
        this.pauseDuration = pauseDuration;
    }

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
        yield return new WaitForSeconds(startDelay);
        owner.Agent.isStopped = false;

        // 처음 플레이어 위치로 이동
        owner.MoveTo(owner.LastKnownTargetPos);
        investigateTimer = 0f;

        while (investigateTimer < investigateDuration)
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

            yield return new WaitForSeconds(pauseDuration);
        }
    }

    // 조사할 랜덤 지점 생성
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
