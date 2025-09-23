using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GoogleSpreadSheetConfig", menuName = "Configs/GoogleSpreadSheetConfig")]
public class GoogleSpreadSheetConfig : ScriptableObject
{
    public List<GoogleSpreadSheetData> datas;
}
