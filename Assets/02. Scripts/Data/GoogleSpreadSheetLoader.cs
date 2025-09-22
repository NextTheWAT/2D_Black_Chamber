using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GoogleSpreadSheetLoader : MonoBehaviour
{
    public string startCell = "A1";
    public string endCell = "C10";
    public string mainURL = "https://docs.google.com/spreadsheets/d/1uI-9xo0u57DOf1XYX-oInEzX1HNYdYHlMcPgnsXe1cc";
    public string gid = "0"; // 기본 시트의 GID
    public string assetName = "SheetData"; // 저장될 SO 이름

    public string URL => mainURL + Format + Range + GID;
    public string Format => "/export?format=tsv";
    public string Range => "&range=" + startCell + ":" + endCell;
    public string GID => "&gid=" + gid;

    private void Start()
    {
        StartCoroutine(DownloadSheet());
    }

    IEnumerator DownloadSheet()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            yield break;
        }

        string text = unityWebRequest.downloadHandler.text;
        string[] rows = text.Split('\n');

        int rowCount = rows.Length;
        int colCount = rows[0].Split('\t').Length;
        string[,] cells = new string[rowCount, colCount];

        for (int r = 0; r < rowCount; r++)
        {
            string[] cols = rows[r].Split('\t');
            for (int c = 0; c < colCount; c++)
            {
                cells[r, c] = cols[c];
            }
        }

#if UNITY_EDITOR
        // ScriptableObject 생성 및 저장
        Sheet sheetAsset = ScriptableObject.CreateInstance<Sheet>();
        sheetAsset.SetData(cells);

        string assetPath = $"Assets/06. ScriptableObjects/{assetName}.asset";
        AssetDatabase.CreateAsset(sheetAsset, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Sheet ScriptableObject 저장 완료: {assetPath}");
#endif
    }
}
