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
        // '수색' 대사 출력 로직 수정 (ScriptableObject 기반)
        // EnemyState.Investigate 상태의 랜덤 대사를 가져옴
        string dialogue = owner.GetRandomDialogue(EnemyState.Investigate);
        owner.DisplayDialogue(dialogue);

        BeginInvestigate();
        ConditionalLogger.Log("InvestigateState Enter");
    }

    public override void Exit()
    {
        StopInvestigate();
        ConditionalLogger.Log("InvestigateState Exit");
    }

    public override void Update()
    {
        // 소음이 계속 감지되면 조사 상태를 재시작
        if(owner.IsNoiseDetected && owner.NoiseAmount > NoiseManager.Instance.InvestigateThreshold)
        {
            Debug.Log("Reinvestigating due to continuous noise detection.");
            StopInvestigate();
            BeginInvestigate();
        }
    }


    private void BeginInvestigate()
    {
        investigateTimer = 0f;
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

        investigateTimer = 0f;
        owner.investigateUseStartDelay = false;
        owner.Agent.isStopped = false;
        owner.AutoRotate = false;
    }

    private IEnumerator InvestigateLoop()
    {
        Vector2 dirToLastKnown = (owner.LastKnownTargetPos - (Vector2)owner.transform.position).normalized;
        owner.LookPoint = (Vector2)owner.transform.position + dirToLastKnown;
        owner.Agent.isStopped = true;

        // LastKnownTargetPos 방향으로 회전 대기
        while (Mathf.Abs(owner.CurrentLookAngleDelta) > 1f)
            yield return null;

        // 조사 시작 전 대기 (바로 조사 모드가 아니면)
        if (!owner.investigateUseStartDelay)
        {
            owner.investigateUseStartDelay = false;
            yield return new WaitForSeconds(owner.Data.investigateStartDelay);
        }

        // 처음 플레이어 위치로 이동
        owner.AutoRotate = true;
        owner.Agent.isStopped = false;
        owner.MoveTo(owner.LastKnownTargetPos);
        investigateTimer = 0f;

        while (IsInvestigating)
        {
            if (owner.IsArrived) break;
            investigateTimer += Time.deltaTime;
            yield return null;
        }

        while (IsInvestigating)
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
