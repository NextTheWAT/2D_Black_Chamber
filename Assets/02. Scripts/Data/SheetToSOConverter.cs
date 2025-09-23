#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public static class SheetToSOConverter
{
    public static void ConvertEnemySheet(Sheet sheet, string folderPath)
    {
        if (sheet == null) return;

        // 첫 행을 헤더로 사용
        var headers = sheet.GetRow(0);

        for (int r = 1; r < sheet.RowCount; r++)
        {
            EnemySheetData data = ScriptableObject.CreateInstance<EnemySheetData>();

            for (int c = 0; c < sheet.ColumnCount; c++)
            {
                string key = headers[c];
                string value = sheet[r, c];
                
                switch (key)
                {
                    case "enemyName": data.enemyName = value; break;
                    case "hp": int.TryParse(value, out data.hp); break;
                    case "speed": float.TryParse(value, out data.speed); break;
                    case "viewDistance": float.TryParse(value, out data.viewDistance); break;
                    case "viewAngle": float.TryParse(value, out data.viewAngle); break;
                    case "equipWeapon": int.TryParse(value, out data.equipWepaon); break;
                    case "attackRange": float.TryParse(value, out data.attackRange); break;
                    case "patrolType": int.TryParse(value, out data.patrolType); break;
                    case "patrolPauseTime": float.TryParse(value, out data.patrolPauseTime); break;
                    case "FixedPatrolAngle": float.TryParse(value, out data.FixedPatrolAngle); break;
                    case "suspectTime": float.TryParse(value, out data.suspectTime); break;
                    case "investigateDuration": float.TryParse(value, out data.investigateDuration); break;
                    case "investigateRange": float.TryParse(value, out data.investigateRange); break;
                }
            }

            string path = $"{folderPath}/{data.enemyName}.asset";
            AssetDatabase.CreateAsset(data, path);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[SheetToSO] {sheet.RowCount - 1}개의 EnemySheetData 생성 완료!");
    }
}
#endif