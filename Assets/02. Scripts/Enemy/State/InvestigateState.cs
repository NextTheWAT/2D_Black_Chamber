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
        // ó�� �÷��̾� ��ġ�� �̵�
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
            // ������ ���� �������� �̵�
            do
            {
                owner.MoveTo(GetRandomInvestigatePoint());
                owner.investigateTimer += Time.deltaTime;
                yield return null;
            }
            while (!owner.Agent.hasPath);

            // �������� ������ ������ ���
            while (!owner.IsArrived)
            {
                owner.investigateTimer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    // ������ ���� ���� ����
    private Vector2 GetRandomInvestigatePoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * owner.investigateRange;
        Vector2 investigatePoint = (Vector2)owner.transform.position + randomDirection;
        return investigatePoint;
    }
}
