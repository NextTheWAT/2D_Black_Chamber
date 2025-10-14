using UnityEngine;

//
// Circle �� Sector : �ݰ� + ������ ���� ������ �����ϴ� ���.
// - radius: �ִ� �Ÿ�(�� �ݰ�)
// - angleDeg: �¿� �� �þ߰� (�߽��� ������ forward)
// - edgeSoftness: �ݰ� ��� �ε巯��(���� ����)
// - angleSoftnessDeg: �� ��� �ε巯��(�� ����)
//
[CreateAssetMenu(menuName = "FogOfWar/Shape/Circle+Sector", fileName = "CircleSectorShape")]
public class CircleSectorShapeAsset : RevealerShapeAsset
{
    [Header("Distance (Circle)")]
    public float radius = 6f;
    [Range(0f, 2f)] public float edgeSoftness = 0.75f;     // ��� ���̵�(�Ÿ�)

    [Header("Direction (Sector)")]
    [Range(0f, 360f)] public float angleDeg = 90f;         // �� �þ߰�(��angle/2)
    [Range(0f, 45f)] public float angleSoftnessDeg = 5f;   // ��� ���̵�(����)

    public override float Evaluate(Vector2 localPoint, Vector2 forward)
    {
        if (forward.sqrMagnitude < 1e-6f) forward = Vector2.right;

        // 1) �ݰ� ����ġ (��)
        float d = localPoint.magnitude;
        float radial = Mathf.Clamp01(1f - Mathf.InverseLerp(radius - edgeSoftness, radius, d));
        if (radial <= 0f) return 0f;

        // 2) ���� ����ġ (��ä��)
        float half = angleDeg * 0.5f;
        float ang = Vector2.Angle(forward.normalized, localPoint);

        if (angleSoftnessDeg <= 1e-4f)
        {
            // ��ī�ο� ���
            if (ang > half) return 0f;
            return radial;
        }
        else
        {
            // �ε巯�� ���: [half - s, half + s] �������� 1��0 ���� ���̵�
            float s = angleSoftnessDeg;
            // InverseLerp(a,b,x): x<=a ->1, x>=b ->0�� �ǵ��� a>b�� �־� ������
            float angWeight = Mathf.Clamp01(Mathf.InverseLerp(half + s, half - s, ang));
            return radial * angWeight;
        }
    }

    public override Bounds GetLocalBounds()
    {
        // Ÿ�� ���� ���� ��꿡 ���̴� ���� AABB (�� ����)
        return new Bounds(Vector3.zero, new Vector3(radius * 2f, radius * 2f, 0f));
    }
}
