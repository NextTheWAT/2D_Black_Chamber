using UnityEngine;

[CreateAssetMenu(menuName = "FogOfWar/Shape/TextureMask", fileName = "TextureMaskShape")]
public class TextureMaskShapeAsset : RevealerShapeAsset
{
    public Texture2D mask; // grayscale; white=open, black=closed
    [Range(0f, 1f)] public float alphaCutoff = 0.01f; // below this returns 0
    public Vector2 pivot01 = new Vector2(0.5f, 0.5f); // (0,0)=bottom-left


    private Color[] cache; // cached pixels
    private int w, h;


    private void OnEnable()
    {
        RebuildCache();
    }


    public void RebuildCache()
    {
        if (mask == null) { cache = null; return; }
        w = mask.width; h = mask.height;
        cache = mask.GetPixels();
    }


    public override float Evaluate(Vector2 p, Vector2 forward)
    {
        if (cache == null || w == 0 || h == 0) return 0f;
        // map p(x,y) to UV in [0..1] assuming local bounds approximate [-0.5..0.5] * size; we choose 1 unit == 1 pixel here
        float u = (p.x + (w * (0.5f - pivot01.x)) + 0.5f) / w;
        float v = (p.y + (h * (0.5f - pivot01.y)) + 0.5f) / h;
        if (u < 0f || u > 1f || v < 0f || v > 1f) return 0f;
        float x = Mathf.Clamp(u * (w - 1), 0, w - 1);
        float y = Mathf.Clamp(v * (h - 1), 0, h - 1);
        int xi = Mathf.FloorToInt(x), yi = Mathf.FloorToInt(y);
        int idx = yi * w + xi;
        float a = cache[idx].grayscale;
        if (a < alphaCutoff) return 0f;
        return a;
    }


    public override Bounds GetLocalBounds()
    {
        if (mask == null) return new Bounds(Vector3.zero, Vector3.one);
        return new Bounds(Vector3.zero, new Vector3(mask.width, mask.height, 0f));
    }
}