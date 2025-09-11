using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FOVUtility
{
    public static Transform FindTargetInFOV(this Enemy self)
        => FindTargetInFOV(self.transform, self.ViewAngle, self.ViewDistance, self.TargetMask, self.ObstacleMask);


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


    public static bool IsCircleVisible(this Transform self, CircleCollider2D circle, float viewAngle, float viewDistance, LayerMask targetMask, LayerMask obstacleMask)
    {
        Vector2 originPos = self.position;
        Vector2 center = circle.bounds.center;
        float radius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);

        Vector2 dirToCenter = (center - originPos).normalized;
        float distToCenter = Vector2.Distance(originPos, center);

        // 원 반지름으로 인한 시야각 확장
        float radiusAngle = 0f;
        if (distToCenter > 0f)
            radiusAngle = Mathf.Asin(Mathf.Clamp(radius / distToCenter, -1f, 1f)) * Mathf.Rad2Deg;

        float angleToCenter = Vector2.Angle(self.up, dirToCenter);

        // 시야각 + 거리 체크
        if (angleToCenter - radiusAngle <= viewAngle / 2f && distToCenter - radius <= viewDistance)
        {
            if (!Physics2D.Raycast(originPos, dirToCenter, distToCenter, obstacleMask))
                return true;
        }

        return false;
    }
}
