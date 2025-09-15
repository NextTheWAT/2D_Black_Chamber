using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FOVUtility
{
    public static Transform FindTargetInFOV(this Enemy self)
        => FindTargetInFOV(self.transform, self.ViewAngle, self.ViewDistance, self.TargetMask, self.ObstacleMask);

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
        Vector2 dirToPoint = (point - (Vector2)self.position).normalized;
        float distToPoint = Vector2.Distance(self.position, point);
        float angleToPoint = Vector2.Angle(self.up, dirToPoint);
        float halfViewAngle = viewAngle * 0.5f;

        if (angleToPoint <= halfViewAngle && distToPoint <= viewDistance)
        {
            RaycastHit2D hit = Physics2D.Raycast(self.position, dirToPoint, distToPoint, obstacleMask);
            if (hit.collider != null && hit.collider.transform != self)
                return false;
            else
                return true;
        }

        return false;
    }

}
