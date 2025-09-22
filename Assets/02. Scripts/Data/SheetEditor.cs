// SheetEditor.cs
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Sheet))]
public class SheetEditor : Editor
{
    private Vector2 scroll;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Sheet sheet = (Sheet)target;
        if (sheet.cells == null)
        {
            EditorGUILayout.HelpBox("cells 배열이 비어있습니다.", MessageType.Info);
            return;
        }

        int rows = sheet.RowCount;
        int cols = sheet.ColumnCount;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int r = 0; r < rows; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < cols; c++)
            {
                string oldValue = sheet.cells[r, c];
                string newValue = EditorGUILayout.TextField(oldValue, GUILayout.MinWidth(50));

                if (newValue != oldValue)
                {
                    sheet.cells[r, c] = newValue;
                    EditorUtility.SetDirty(sheet);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif
