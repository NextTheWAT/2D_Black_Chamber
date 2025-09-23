
[System.Serializable]
public class GoogleSpreadSheetData
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
}