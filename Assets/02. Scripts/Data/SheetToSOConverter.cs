#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Constants;

public static class SheetToSOConverter
{
    public static void ConvertSOData(Sheet sheet, SheetType type, string folderPath)
    {
        if (sheet == null) return;

        // ù ���� ���
        var headers = sheet.GetRow(0);

        for (int r = 1; r < sheet.RowCount; r++)
        {
            EnemySheetData data = ScriptableObject.CreateInstance<EnemySheetData>();

            for (int c = 0; c < sheet.ColumnCount; c++)
            {
                string key = headers[c];
                string value = sheet[r, c];

                if (string.IsNullOrWhiteSpace(key)) continue;

                // EnemySheetData�� �ʵ� ��������
                FieldInfo field = typeof(EnemySheetData).GetField(key, BindingFlags.Public | BindingFlags.Instance);
                if (field == null) continue; // ���� �ʵ�� ����

                try
                {
                    object parsedValue = ParseValue(value, field.FieldType);
                    field.SetValue(data, parsedValue);
                }
                catch (Exception ex)
                {
                    ConditionalLogger.LogWarning($"[SheetToSO] {key} ��ȯ ���� ({value}) �� {ex.Message}");
                }
            }

            string assetPath = $"{folderPath}/{data.enemyName}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        ConditionalLogger.Log($"[SheetToSO] {sheet.RowCount - 1}���� EnemySheetData ���� �Ϸ�!");
    }

    private static ScriptableObject CreateSOInstance(SheetType type)
    {
        return type switch
        {
            SheetType.Enemy => ScriptableObject.CreateInstance<EnemySheetData>(),
            // �ٸ� Ÿ�� �߰� ����
            _ => null,
        };
    }

    private static object ParseValue(string value, Type fieldType)
    {
        if (fieldType == typeof(string)) return value;
        if (fieldType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (fieldType == typeof(float)) return float.TryParse(value, out var f) ? f : 0f;
        if (fieldType == typeof(bool)) return bool.TryParse(value, out var b) && b;

        // enum ����
        if (fieldType.IsEnum && Enum.TryParse(fieldType, value, true, out var e))
            return e;

        return null;
    }
}
#endif
