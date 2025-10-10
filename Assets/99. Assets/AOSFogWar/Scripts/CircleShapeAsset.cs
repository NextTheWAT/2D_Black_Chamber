using UnityEngine;

[CreateAssetMenu(menuName = "FogOfWar/Shape/Circle", fileName = "CircleShape")]
public class CircleShapeAsset : RevealerShapeAsset
{
    public float radius = 6f;
    [Range(0f, 2f)] public float edgeSoftness = 0.75f; // units of smooth fade at boundary


    public override float Evaluate(Vector2 p, Vector2 forward)
    {
        float d = p.magnitude;
        float t0 = Mathf.Clamp01(1f - Mathf.InverseLerp(radius - edgeSoftness, radius, d));
        return t0; // center=1, boundary=0
    }


    public override Bounds GetLocalBounds() => new Bounds(Vector3.zero, new Vector3(radius * 2f, radius * 2f, 0f));
}