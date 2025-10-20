#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    public static void SaveScriptableObject<T>(T data, string path) where T : ScriptableObject
    {
        if (data == null)
        {
            Debug.LogError("저장할 ScriptableObject가 null입니다.");
            return;
        }

        string directory = Path.GetDirectoryName(path);

        // 폴더가 없으면 생성
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // 기존 에셋 확인
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (existingAsset)
        {
            // 값만 덮어쓰기
            EditorUtility.CopySerialized(data, existingAsset);
            EditorUtility.SetDirty(existingAsset);
        }
        else
        {
            // 새로 생성
            AssetDatabase.CreateAsset(data, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
