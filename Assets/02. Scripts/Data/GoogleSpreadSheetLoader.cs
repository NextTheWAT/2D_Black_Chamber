#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class GoogleSpreadSheetLoader : EditorWindow
{
    private GoogleSpreadSheetConfig config;
    private SerializedObject serializedConfig;

    private const string PREF_KEY = "GoogleSpreadSheetLoader_LastConfigPath";

    [MenuItem("Tools/Google Sheet Loader")]
    public static void ShowWindow()
    {
        var window = GetWindow<GoogleSpreadSheetLoader>("Google Sheet Loader");

        // EditorPrefs에서 마지막 사용 Config 불러오기
        string lastPath = EditorPrefs.GetString(PREF_KEY, "");
        if (!string.IsNullOrEmpty(lastPath))
        {
            var lastConfig = AssetDatabase.LoadAssetAtPath<GoogleSpreadSheetConfig>(lastPath);
            if (lastConfig != null)
            {
                window.SetConfig(lastConfig);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet Loader", EditorStyles.boldLabel);

        var newConfig = (GoogleSpreadSheetConfig)EditorGUILayout.ObjectField("Config", config, typeof(GoogleSpreadSheetConfig), false);
        if (newConfig != config)
        {
            SetConfig(newConfig);
        }

        if (config == null)
        {
            EditorGUILayout.HelpBox("GoogleSpreadSheetConfig.asset 을 선택하세요.", MessageType.Info);
            return;
        }

        serializedConfig.Update();

        // datas 리스트 직접 그리기
        SerializedProperty datasProp = serializedConfig.FindProperty("datas");
        for (int i = 0; i < datasProp.arraySize; i++)
        {
            SerializedProperty element = datasProp.GetArrayElementAtIndex(i);
            SerializedProperty assetNameProp = element.FindPropertyRelative("assetName");
            SerializedProperty startCellProp = element.FindPropertyRelative("startCell");
            SerializedProperty endCellProp = element.FindPropertyRelative("endCell");
            SerializedProperty mainURLProp = element.FindPropertyRelative("mainURL");
            SerializedProperty gidProp = element.FindPropertyRelative("gid");
            SerializedProperty typeProp = element.FindPropertyRelative("sheetType");
            SerializedProperty pathProp = element.FindPropertyRelative("path");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(assetNameProp);
            EditorGUILayout.PropertyField(startCellProp);
            EditorGUILayout.PropertyField(endCellProp);
            EditorGUILayout.PropertyField(mainURLProp);
            EditorGUILayout.PropertyField(gidProp);
            EditorGUILayout.PropertyField(typeProp);

            // path는 버튼으로만 설정
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Path", GUILayout.Width(70));
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(pathProp.stringValue) ? "Not Set" : pathProp.stringValue, GUILayout.Height(16));

            if (GUILayout.Button("Select Folder", GUILayout.Width(120)))
            {
                string selected = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected.StartsWith(Application.dataPath))
                        selected = "Assets" + selected.Substring(Application.dataPath.Length);

                    pathProp.stringValue = selected;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        serializedConfig.ApplyModifiedProperties();
        EditorUtility.SetDirty(config);

        GUILayout.Space(10);

        if (GUILayout.Button("Download All"))
        {
            for (int i = 0; i < config.datas.Count; i++)
            {
                var data = config.datas[i];
                if (string.IsNullOrEmpty(data.assetName) || string.IsNullOrEmpty(data.path))
                {
                    Debug.LogWarning($"[GoogleSheetLoader] {data.assetName} : 저장 경로가 설정되지 않음.");
                    continue;
                }
                DownloadSheetSync(data);
            }

        }
    }

    private void SetConfig(GoogleSpreadSheetConfig newConfig)
    {
        config = newConfig;
        serializedConfig = config != null ? new SerializedObject(config) : null;

        if (config != null)
        {
            string path = AssetDatabase.GetAssetPath(config);
            EditorPrefs.SetString(PREF_KEY, path); // 마지막 선택 Config 저장
        }
    }

    private void DownloadSheetSync(GoogleSpreadSheetData data)
    {
        using var request = UnityWebRequest.Get(data.URL);
        request.SendWebRequest();
        while (!request.isDone) { }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[GoogleSheetLoader] {data.assetName} : {request.error}");
            return;
        }

        string[,] cells = ParseTSV(request.downloadHandler.text);
        var sheetAsset = CreateInstance<Sheet>();
        sheetAsset.SetData(cells);

        string assetPath = $"{data.path}/{data.assetName}.asset";
        AssetDatabase.CreateAsset(sheetAsset, assetPath);
        AssetDatabase.SaveAssets();

        SheetToSOConverter.ConvertSOData(sheetAsset, data.sheetType, data.path);
        Debug.Log($"[GoogleSheetLoader] 저장 완료: {assetPath}");
    }

    private string[,] ParseTSV(string text)
    {
        var rows = text.Split('\n');
        int rowCount = rows.Length;
        int colCount = rows[0].Split('\t').Length;
        var cells = new string[rowCount, colCount];

        for (int r = 0; r < rowCount; r++)
        {
            var cols = rows[r].Split('\t');
            for (int c = 0; c < cols.Length; c++)
                cells[r, c] = cols[c];
        }
        return cells;
    }
}
#endif
