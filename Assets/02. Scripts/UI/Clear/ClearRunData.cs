using UnityEngine;

public class ClearRunData : MonoBehaviour
{
    public static string lastGameplayScene = "ProtoTypeScene"; //재시도 시 불러올 씬
    public static int Kills = 0;
    public static string clearState = "잠입 상태 클리어"; //잠입 & 난전
    public static float elapsedTime = 0f;
    public static int reward = 0;
}
