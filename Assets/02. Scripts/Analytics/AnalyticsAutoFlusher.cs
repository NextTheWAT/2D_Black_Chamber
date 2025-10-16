using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsAutoFlusher : MonoBehaviour
{
#if UNITY_WEBGL
    [SerializeField] float interval = 15f;
    void OnEnable() => StartCoroutine(Loop());
    IEnumerator Loop()
    {
        while (true) { yield return new WaitForSeconds(interval); AnalyticsService.Instance.Flush(); }
    }
#endif
}
