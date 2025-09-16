#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class SpotLightMaskGenerator : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private Color fillColor = Color.red; // R 채널용
    [SerializeField] private int textureSize = 256;
    [SerializeField] private float offset = 0f;

    [ContextMenu("Generate Multiply Mask (R) Sprite With Alpha")]
    void GenerateMaskSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float outerRadius = light2D.pointLightOuterRadius;
        float innerRadius = light2D.pointLightInnerRadius;
        float outerAngle = light2D.pointLightOuterAngle * Mathf.Deg2Rad;
        float innerAngle = light2D.pointLightInnerAngle * Mathf.Deg2Rad;
        float halfOuter = outerAngle / 2f;
        float halfInner = innerAngle / 2f;

        Texture2D tex = new(textureSize, textureSize, TextureFormat.ARGB32, false);
        Color32 clear = new(0, 0, 0, 0);
        Vector2 center = new(textureSize / 2f, textureSize / 2f);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 dir = Quaternion.Euler(0, 0, offset) * new Vector2(x - center.x, y - center.y);
                float dist = dir.magnitude / (textureSize / 2f) * outerRadius;
                float ang = Mathf.Atan2(dir.y, dir.x);

                if (dist <= outerRadius && Mathf.Abs(ang) <= halfOuter)
                {
                    // 반지름 기준 알파
                    float radialAlpha = 1f;
                    if (dist > innerRadius)
                        radialAlpha = 1f - (dist - innerRadius) / (outerRadius - innerRadius);

                    // 각도 기준 알파
                    float angleAlpha = 1f;
                    if (Mathf.Abs(ang) > halfInner)
                        angleAlpha = 1f - (Mathf.Abs(ang) - halfInner) / (halfOuter - halfInner);

                    float finalAlpha = Mathf.Clamp01(light2D.intensity * radialAlpha * angleAlpha);
                    Color32 fill = new Color32(
                        (byte)(fillColor.r * 255),
                        (byte)(fillColor.g * 255),
                        (byte)(fillColor.b * 255),
                        (byte)(finalAlpha * 255)
                    );
                    tex.SetPixel(x, y, fill);
                }
                else
                {
                    tex.SetPixel(x, y, clear);
                }
            }
        }

        tex.Apply();

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize / (outerRadius * 2f)
        );
        sprite.name = "MultiplyMask_R";
        spriteRenderer.sprite = sprite;

#if UNITY_EDITOR
        byte[] bytes = tex.EncodeToPNG();
        string path = "Assets/07. Sprites(Image)/MultiplyMask_R.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = textureSize / (outerRadius * 2f);
            ti.alphaIsTransparency = true;
            ti.SaveAndReimport();
        }
#endif
    }
}
