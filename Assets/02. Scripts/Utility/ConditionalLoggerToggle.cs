using UnityEngine;
using UnityEditor;
using System.Linq;

public class ConditionalLoggerToggle : MonoBehaviour
{
    private const string Define = "DEBUG_LOG";

#if UNITY_EDITOR
    [MenuItem("Tools/Enable Debug Log")]
    private static void EnableDebugLog()
    {
        SetDebugLogSymbol(true);
        Debug.Log("Debug Log Enabled");
    }

    [MenuItem("Tools/Disable Debug Log")]
    private static void DisableDebugLog()
    {
        SetDebugLogSymbol(false);
        Debug.Log("Debug Log Disabled");
    }

    private static void SetDebugLogSymbol(bool enable)
    {
        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(";").ToList();

        if (enable && !defines.Contains(Define))
            defines.Add(Define);
        else if(!enable)
            defines.RemoveAll(d => d == Define);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines));
    }
#endif
}
