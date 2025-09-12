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

        // ó�� �÷��̾� ��ġ�� �̵�
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
            // ������ ���� �������� �̵�
            do
            {
                GetRandomInvestigatePoint();
                owner.MoveTo(GetRandomInvestigatePoint());
                timer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // �������� ������ ������ ���
            while (owner.Agent.remainingDistance > 0.01f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }

        // ���� �ð��� ������ ���� ���·� ��ȯ
        owner.ChangeState<ReturnState>();
    }

    // ������ ���� ���� ����
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
