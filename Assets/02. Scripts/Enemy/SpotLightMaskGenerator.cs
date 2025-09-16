using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class SpotLightSpriteGenerator : MonoBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private int textureSize = 256; // 스프라이트 해상도
    [SerializeField] private float offset = 0f; // 스프라이트 회전 오프셋

    [ContextMenu("Generate Sector Sprite")]
    void GenerateSectorSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float radius = light2D.pointLightOuterRadius;
        float angle = light2D.pointLightOuterAngle * Mathf.Deg2Rad;
        float halfAngle = angle / 2f;

        // 텍스처 준비 (검은 배경, 흰색 부채꼴)
        Texture2D tex = new(textureSize, textureSize, TextureFormat.ARGB32, false);
        Color32 clear = new(0, 0, 0, 0);
        Color32 fill = new(255, 255, 255, 255);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                tex.SetPixel(x, y, clear);
            }
        }

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
                }
            }
        }

        tex.Apply();

        // 텍스처 → 스프라이트
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize / (radius * 2f) // 픽셀 퍼 유닛
        );
        sprite.name = "SpotLightSector";
        spriteRenderer.sprite = sprite;
    }
}
