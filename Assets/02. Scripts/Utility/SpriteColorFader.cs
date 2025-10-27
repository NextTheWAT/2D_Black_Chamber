using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorFader : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    public Color startColor;
    public Color endColor;
    public float fadeDuration = 1f;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartFade()
    {
        StopFade();
        fadeCoroutine = StartCoroutine(FadeCoroutine());
    }

    public void StopFade()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        foreach (var sr in spriteRenderers)
            sr.color = endColor;
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            Color currentColor = Color.Lerp(startColor, endColor, t);

            foreach (var sr in spriteRenderers)
                sr.color = currentColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var sr in spriteRenderers)
            sr.color = endColor;

        fadeCoroutine = null;
    }


}
