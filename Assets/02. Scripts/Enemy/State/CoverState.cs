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

        // 1. NavMesh 경로 기반 후보 얻기
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
            // 2. 플레이어와의 Linecast로 가려지는지 확인
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

        // 3. 마지막으로 가려진 코너 뒤로 숨기
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
                // 벽 안쪽 깊이
                float coverDepth = 0.5f;

                // 벽 표면 tangent
                Vector2 tangent = new(-hit.normal.y, hit.normal.x);

                // 플레이어 반대 방향
                Vector2 dirToPlayer = ((Vector2)owner.Target.position - hit.point).normalized;
                Vector2 oppositeDir = -dirToPlayer;

                // 벽 안쪽으로 이동
                Vector2 inward = hit.point + hit.normal * coverDepth;

                // 벽을 따라 플레이어 반대 방향으로 이동
                Vector2 tangentMove = tangent * Vector2.Dot(oppositeDir, tangent) * coverOffset;
                Vector2 finalCoverPoint = inward + tangentMove;

                // NavMesh 위로 보정
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
