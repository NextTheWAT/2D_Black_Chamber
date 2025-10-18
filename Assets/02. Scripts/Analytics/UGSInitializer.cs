using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;
using UnityEngine;

public class UGSInitializer : MonoBehaviour
{
    [SerializeField] bool useProduction = true; // itch 배포용 true
    async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        var env = useProduction ? "production" : "ProtoType_1";
        var opts = new InitializationOptions().SetEnvironmentName(env);
        await UnityServices.InitializeAsync(opts);
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log($"UGS Initialized ({env})");
    }

#if UNITY_WEBGL
    void OnApplicationPause(bool pause)
    {
        if (pause) AnalyticsService.Instance.Flush();
    }
    void OnApplicationQuit()
    {
        AnalyticsService.Instance.Flush();
    }
#endif
}
//WebGL은 탭 닫힘/포커스 전환 때 유실될 수 있어 Flush를 자주 호출하는 게 안전하다.
