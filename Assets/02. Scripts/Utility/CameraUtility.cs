using System.Collections;
using UnityEngine;
using Cinemachine;


public static class CameraUtility
{
    private static CinemachineVirtualCamera cachedVcam;
    private static CinemachineBasicMultiChannelPerlin perlin;
    private static Coroutine shakeCoroutine;

    public static void Shake(float amplitude)
    {
        if (cachedVcam == null)
        {
            cachedVcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            if (cachedVcam == null)
            {
                Debug.LogWarning("CameraUtility.Shake: No CinemachineVirtualCamera found in the scene.");
                return;
            }
        }

        if (perlin == null)
        {
            perlin = cachedVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin == null)
            {
                perlin = cachedVcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_NoiseProfile = Resources.Load<NoiseSettings>("NoiseProfiles/6D Shake");
                perlin.m_FrequencyGain = 10f;
            }
        }

        if (shakeCoroutine != null)
            cachedVcam.StopCoroutine(shakeCoroutine);

        shakeCoroutine = cachedVcam.StartCoroutine(ShakeCoroutine(amplitude, 0.5f));
    }
    private static IEnumerator ShakeCoroutine(float amplitude, float fadeTime)
    {
        float elapsed = 0f;
        perlin.m_AmplitudeGain = amplitude;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);
            perlin.m_AmplitudeGain = Mathf.Lerp(amplitude, 0f, t);
            yield return null;
        }

        perlin.m_AmplitudeGain = 0f;
    }
}
