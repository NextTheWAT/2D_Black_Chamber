using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class CoverState : BaseState
{
    private LayerMask obstacleLayer;
    private float coverOffset = 2f;
    private Vector2 lookPoint;

    public CoverState(Enemy owner, LayerMask obstacleLayer, float coverOffset) : base(owner)
    {
        this.obstacleLayer = obstacleLayer;
        this.coverOffset = coverOffset;
    }

    public override void Enter()
    {
        ConditionalLogger.Log("CoverState Enter");
        owner.Target = GameManager.Instance.player;
        Vector2 coverPoint = GetCoverPoint();
        if(Vector2.Distance(owner.transform.position, coverPoint) > 1f)
            owner.MoveTo(coverPoint);
    }

    public override void Update()
    {
        if (owner.IsArrived)
        {
            owner.LookAt(lookPoint);
        }
    }

    public override void Exit()
    {
        ConditionalLogger.Log("CoverState Exit");
    }

    private Vector2 GetCoverPoint()
    {
        if (!owner.HasTarget) return owner.transform.position;

        // 1. NavMesh ��� ��� �ĺ� ���
        Vector3[] corners = GetPathCorners();
        if (corners == null || corners.Length == 0)
        {
            Debug.LogWarning("No path corners found, returning current position as cover point.");
            return owner.transform.position;
        }

        Vector2 coverPoint = owner.transform.position;
        Vector2? lastBlockedCorner = null;
        Vector2? lastOpenedCorner = null;

        foreach (var corner in corners)
        {
            // 2. �÷��̾���� Linecast�� ���������� Ȯ��
            bool isBlocked = Physics2D.Linecast(corner, owner.Target.position, obstacleLayer);

            if (isBlocked)
            {
                lastBlockedCorner = corner;
            }
            else
            {
                lastOpenedCorner = corner;
                if (lastBlockedCorner == null)
                    return owner.transform.position;
                break;
            }
        }

        if (!lastBlockedCorner.HasValue || !lastOpenedCorner.HasValue)
            return owner.transform.position;

        // 3. ���������� ������ �ڳ� �ڷ� ����
        Vector2 start = (Vector2)lastOpenedCorner;
        Vector2 end = (Vector2)lastBlockedCorner;
        float t = 0f;

        lookPoint = start;

        while (t < 1f)
        {
            t += 0.1f;
            Vector2 point = Vector2.Lerp(start, end, t);
            RaycastHit2D hit = Physics2D.Linecast(point, owner.Target.position, obstacleLayer);

            if (hit.collider)
            {
                // �� ���� ����
                float coverDepth = 0.5f;

                // �� ǥ�� tangent
                Vector2 tangent = new(-hit.normal.y, hit.normal.x);

                // �÷��̾� �ݴ� ����
                Vector2 dirToPlayer = ((Vector2)owner.Target.position - hit.point).normalized;
                Vector2 oppositeDir = -dirToPlayer;

                // �� �������� �̵�
                Vector2 inward = hit.point + hit.normal * coverDepth;

                // ���� ���� �÷��̾� �ݴ� �������� �̵�
                Vector2 tangentMove = tangent * Vector2.Dot(oppositeDir, tangent) * coverOffset;
                Vector2 finalCoverPoint = inward + tangentMove;

                // NavMesh ���� ����
                if (NavMesh.SamplePosition(finalCoverPoint, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
                    finalCoverPoint = navHit.position;

                coverPoint = finalCoverPoint;
                break;
            }
        }

        return coverPoint;
    }

    private Vector3[] GetPathCorners()
    {
        NavMeshPath path = new();
        if (NavMesh.CalculatePath((Vector2)owner.transform.position, (Vector2)owner.Target.position, NavMesh.AllAreas, path))
            return path.corners;
        return null;
    }

}
