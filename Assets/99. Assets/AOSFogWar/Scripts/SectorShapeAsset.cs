using UnityEngine;

[CreateAssetMenu(menuName = "FogOfWar/Shape/Sector", fileName = "SectorShape")]
public class SectorShapeAsset : RevealerShapeAsset
{
    public float innerRadius = 0f;
    public float outerRadius = 7f;
    [Range(0, 360)] public float innerAngleDeg = 0f;
    [Range(0, 360)] public float outerAngleDeg = 90f;


    public override float Evaluate(Vector2 p, Vector2 forward)
    {
        float r = p.magnitude; if (r > outerRadius) return 0f;
        float ang = Vector2.Angle(forward.sqrMagnitude < 1e-5f ? Vector2.right : forward.normalized, p);
        if (ang > outerAngleDeg * 0.5f) return 0f;
        float dist01 = (innerRadius <= 0f) ? 1f : 1f - Mathf.InverseLerp(innerRadius, outerRadius, r);
        float ang01 = (innerAngleDeg <= 0f) ? 1f : 1f - Mathf.InverseLerp(innerAngleDeg * 0.5f, outerAngleDeg * 0.5f, ang);
        return Mathf.Clamp01(dist01 * ang01);
    }


    public override Bounds GetLocalBounds() => new Bounds(Vector3.zero, new Vector3(outerRadius * 2f, outerRadius * 2f, 0f));
}