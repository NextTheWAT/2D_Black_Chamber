using UnityEngine;

[DisallowMultipleComponent]
public class BloodDecal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;     // 프리팹 자식에 있는 SR

    [Header("Sprites (필수)")]
    [SerializeField] private Sprite[] variations;   // 여러 혈흔 스프라이트

    [Header("Randomize")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.2f);
    [SerializeField] private float rotationJitter = 180f;  // ±deg
    [SerializeField] private bool flipX = true;

    [Header("Lifetime")]
    [Tooltip("-1이면 영구 유지")]
    [SerializeField] private float lifetime = -1f;          // seconds
    [SerializeField] private float fadeOut = 0.75f;         // seconds

    Color _origColor;

    void Reset()
    {
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// 적이 죽을 때 호출. 위치와(선택) 기준 각도를 넘겨준다.
    /// </summary>
    public void Setup(Vector3 worldPos, float baseAngleDeg = 0f)
    {
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
        transform.position = worldPos;

        // 스프라이트 선택
        if (variations != null && variations.Length > 0)
            sr.sprite = variations[Random.Range(0, variations.Length)];

        // 회전/스케일/플립
        float jitter = (rotationJitter > 0f) ? Random.Range(-rotationJitter, rotationJitter) : 0f;
        transform.rotation = Quaternion.Euler(0, 0, baseAngleDeg + jitter);

        float s = Random.Range(scaleRange.x, scaleRange.y);
        float sx = (flipX && Random.value < 0.5f) ? -s : s;
        transform.localScale = new Vector3(sx, s, 1f);

        _origColor = sr.color;
        gameObject.SetActive(true);

        // 수명 옵션
        if (lifetime > 0f) StartCoroutine(FadeAndDestroy(lifetime, fadeOut));
    }

    System.Collections.IEnumerator FadeAndDestroy(float wait, float fade)
    {
        yield return new WaitForSeconds(wait);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, fade);
            var c = _origColor; c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
            yield return null;
        }
        Destroy(gameObject);
    }
}
