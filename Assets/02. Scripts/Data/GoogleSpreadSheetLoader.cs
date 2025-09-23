#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class GoogleSpreadSheetLoader : EditorWindow
{
    private GoogleSpreadSheetConfig config;
    private string folderPath = "Assets/06. ScriptableObjects"; // �⺻ ���

    [MenuItem("Tools/Google Sheet Loader")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSpreadSheetLoader>("Google Sheet Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet Loader", EditorStyles.boldLabel);

        // ScriptableObject �巡���ؼ� �Ҵ�
        config = (GoogleSpreadSheetConfig)EditorGUILayout.ObjectField("Config", config, typeof(GoogleSpreadSheetConfig), false);

        if (config == null)
        {
            EditorGUILayout.HelpBox("GoogleSpreadSheetConfig.asset �� �����ϼ���.", MessageType.Info);
            return;
        }

        // datas ����Ʈ ���� �����ϰ� ǥ��
        SerializedObject so = new(config);
        SerializedProperty datasProp = so.FindProperty("datas");
        EditorGUILayout.PropertyField(datasProp, true);
        so.ApplyModifiedProperties();

        GUILayout.Space(10);

        // ���� ���� ��ư
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("���� ����:", folderPath);
        if (GUILayout.Button("Select Folder", GUILayout.Width(120)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // ���� ��θ� ��� ��η� ��ȯ
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogWarning("������ ������Ʈ ���ο� �־�� �մϴ�.");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // �ٿ�ε� ��ư
        if (GUILayout.Button("Download All"))
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets", folderPath.Replace("Assets/", ""));

            foreach (var data in config.datas)
            {
                if (string.IsNullOrEmpty(data.assetName)) continue;
                DownloadSheetSync(data, folderPath);
            }
        }
    }

    private void DownloadSheetSync(GoogleSpreadSheetData data, string path)
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

        string assetPath = $"{path}/{data.assetName}.asset";
        AssetDatabase.CreateAsset(sheetAsset, assetPath);
        AssetDatabase.SaveAssets();
        
        SheetToSOConverter.ConvertEnemySheet(sheetAsset, path);
        Debug.Log($"[GoogleSheetLoader] ���� �Ϸ�: {assetPath}");
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
