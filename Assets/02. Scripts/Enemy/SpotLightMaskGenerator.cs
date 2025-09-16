#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class SpotLightMaskGenerator : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private Color fillColor = Color.white;
    [SerializeField] private int textureSize = 256;
    [SerializeField] private float offset = 0f;

    [ContextMenu("Generate Multiply Mask (R) Sprite")]
    void GenerateMaskSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float radius = light2D.pointLightOuterRadius;
        float angle = light2D.pointLightOuterAngle * Mathf.Deg2Rad;
        float halfAngle = angle / 2f;

        Texture2D tex = new(textureSize, textureSize, TextureFormat.ARGB32, false);
        Color32 clear = new(0, 0, 0, 0);
        Color32 fill = fillColor; // R 채널만 255, G/B 0

        Vector2 center = new(textureSize / 2f, textureSize / 2f);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 dir = Quaternion.Euler(0, 0, offset) * new Vector2(x - center.x, y - center.y);
                float dist = dir.magnitude / (textureSize / 2f) * radius;

                if (dist <= radius)
                {
                    float ang = Mathf.Atan2(dir.y, dir.x);
                    if (ang >= -halfAngle && ang <= halfAngle)
                        tex.SetPixel(x, y, fill);
                    else
                        tex.SetPixel(x, y, clear);
                }
                else
                    tex.SetPixel(x, y, clear);
            }
        }

        tex.Apply();

        // 스프라이트 생성
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize / (radius * 2f)
        );
        sprite.name = "MultiplyMask_R";
        spriteRenderer.sprite = sprite;

#if UNITY_EDITOR
        // PNG로 저장
        byte[] bytes = tex.EncodeToPNG();
        string path = "Assets/07. Sprites(Image)/MultiplyMask_R.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = textureSize / (radius * 2f);
            ti.alphaIsTransparency = true;
            ti.SaveAndReimport();
        }
#endif
    }
}
