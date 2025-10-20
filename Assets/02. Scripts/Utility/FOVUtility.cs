using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FOVUtility
{
    public static Transform FindTargetInFOV(this Transform self, float viewAngle, float viewDistance, LayerMask targetMask, LayerMask obstacleMask)
    {
        Collider2D[] targets = self.FindNearTargets(viewDistance, targetMask);

        foreach (var target in targets)
        {
            if (target.transform == self) continue;
            if (target is not PolygonCollider2D) continue;

            PolygonCollider2D polygonCollider = target as PolygonCollider2D;
            foreach (var point in polygonCollider.points)
            {
                Vector2 worldPoint = polygonCollider.transform.TransformPoint(point);
                if (self.IsPointInFOV(worldPoint, viewAngle, viewDistance, obstacleMask))
                    return target.transform;
            }
        }

        return null;
    }

    /*
    public static Transform FindTargetInFOV(this Transform self, float viewAngle, float viewDistance, LayerMask targetMask, LayerMask obstacleMask)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(self.position, viewDistance, targetMask);
        foreach (var target in targets)
        {
            if (target.transform == self) continue;
            if(target is CircleCollider2D)
            {
                CircleCollider2D circle = target as CircleCollider2D;

                Vector2 dirToTarget = (target.transform.position - self.position).normalized;
                float distToTarget = Vector2.Distance(self.position, target.transform.position);
                float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);

                // 원 반지름으로 인한 시야각 확장
                float radiusAngle = 0f;
                if (distToTarget > 0f)
                    radiusAngle = Mathf.Asin(Mathf.Clamp(radius / distToTarget, -1f, 1f)) * Mathf.Rad2Deg;

                float angleToCenter = Vector2.Angle(self.up, dirToTarget);

                // 시야각 + 거리 체크
                if (angleToCenter - radiusAngle <= viewAngle / 2f && distToTarget - radius <= viewDistance)
                    if (!Physics2D.Raycast(self.position, dirToTarget, distToTarget, obstacleMask))
                        return target.transform;
            }
        }

        return null;
    }
    */

    public static Collider2D[] FindNearTargets(this Transform self, float radius, LayerMask targetMask)
        => Physics2D.OverlapCircleAll(self.position, radius, targetMask);


    public static bool IsPolygonInFOV(this PolygonCollider2D self, float viewAngle, float viewDistance, LayerMask obstacleMask)
    {
        foreach (var point in self.points)
        {
            Vector2 worldPoint = self.transform.TransformPoint(point);
            if (self.transform.IsPointInFOV(worldPoint, viewAngle, viewDistance, obstacleMask))
                return true;
        }

        return false;
    }
    public static bool IsPointInFOV(this Transform self, Vector2 point, float viewAngle, float viewDistance, LayerMask obstacleMask)
    {
        Vector2 selfPos = self.position;
        Vector2 dirToPoint = point - selfPos;

        float distToPoint = dirToPoint.magnitude;
        if (distToPoint > viewDistance) return false;

        float angleToPoint = Vector2.Angle(self.up, dirToPoint);
        float halfViewAngle = viewAngle * 0.5f;
        if (angleToPoint > halfViewAngle) return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(self.position, dirToPoint, distToPoint, obstacleMask);
        foreach (var hit in hits)
        {
            if (hit.collider.transform == self || hit.collider.transform == self.parent) continue; // 자기 자신과 부모는 무시
            if (hit.collider) return false; // 다른 오브젝트에 막힘
        }

        return true; // 시야 확보
    }

}
