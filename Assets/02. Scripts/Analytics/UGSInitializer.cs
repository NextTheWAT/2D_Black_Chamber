using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;
using UnityEngine;

public class UGSInitializer : MonoBehaviour
{
    [SerializeField] bool useProduction = true; // itch ������ true
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
//WebGL�� �� ����/��Ŀ�� ��ȯ �� ���ǵ� �� �־� Flush�� ���� ȣ���ϴ� �� �����ϴ�.
