using UnityEngine;

//
// Circle ∩ Sector : 반경 + 각도로 가시 영역을 제한하는 모양.
// - radius: 최대 거리(원 반경)
// - angleDeg: 좌우 총 시야각 (중심은 리빌러 forward)
// - edgeSoftness: 반경 경계 부드러움(유닛 단위)
// - angleSoftnessDeg: 각 경계 부드러움(도 단위)
//
[CreateAssetMenu(menuName = "FogOfWar/Shape/Circle+Sector", fileName = "CircleSectorShape")]
public class CircleSectorShapeAsset : RevealerShapeAsset
{
    [Header("Distance (Circle)")]
    public float radius = 6f;
    [Range(0f, 2f)] public float edgeSoftness = 0.75f;     // 경계 페이드(거리)

    [Header("Direction (Sector)")]
    [Range(0f, 360f)] public float angleDeg = 90f;         // 총 시야각(±angle/2)
    [Range(0f, 45f)] public float angleSoftnessDeg = 5f;   // 경계 페이드(각도)

    public override float Evaluate(Vector2 localPoint, Vector2 forward)
    {
        if (forward.sqrMagnitude < 1e-6f) forward = Vector2.right;

        // 1) 반경 가중치 (원)
        float d = localPoint.magnitude;
        float radial = Mathf.Clamp01(1f - Mathf.InverseLerp(radius - edgeSoftness, radius, d));
        if (radial <= 0f) return 0f;

        // 2) 각도 가중치 (부채꼴)
        float half = angleDeg * 0.5f;
        float ang = Vector2.Angle(forward.normalized, localPoint);

        if (angleSoftnessDeg <= 1e-4f)
        {
            // 날카로운 경계
            if (ang > half) return 0f;
            return radial;
        }
        else
        {
            // 부드러운 경계: [half - s, half + s] 구간에서 1→0 선형 페이드
            float s = angleSoftnessDeg;
            // InverseLerp(a,b,x): x<=a ->1, x>=b ->0가 되도록 a>b로 넣어 역보간
            float angWeight = Mathf.Clamp01(Mathf.InverseLerp(half + s, half - s, ang));
            return radial * angWeight;
        }
    }

    public override Bounds GetLocalBounds()
    {
        // 타일 샘플 범위 계산에 쓰이는 로컬 AABB (원 기준)
        return new Bounds(Vector3.zero, new Vector3(radius * 2f, radius * 2f, 0f));
    }
}
