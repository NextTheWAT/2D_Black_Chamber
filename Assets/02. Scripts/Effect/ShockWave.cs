using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public float duration;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private SpriteRenderer spriteRenderer;
    private Material instanceMaterial;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        instanceMaterial = Instantiate(spriteRenderer.material);
        spriteRenderer.material = instanceMaterial;
        Play(duration);
    }

    public void Play(float duration)
    {
        StartCoroutine(AnimateShockWave(duration));
    }

    IEnumerator AnimateShockWave(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(0f, 1f, elapsed / duration);
            instanceMaterial.SetFloat(_waveDistanceFromCenter, progress);
            yield return null;
        }

        // 애니메이션이 끝난 후 오브젝트 제거
        Destroy(gameObject);
    }


}
