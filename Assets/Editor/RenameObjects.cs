using UnityEditor;
using UnityEngine;

public class RenameObjects : EditorWindow
{
    string baseName = "MyObject";

    [MenuItem("Tools/Rename Selected Objects")]
    static void Init()
    {
        RenameObjects window = (RenameObjects)EditorWindow.GetWindow(typeof(RenameObjects));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("이름 재정렬 도구", EditorStyles.boldLabel);
        baseName = EditorGUILayout.TextField("기본 이름", baseName);

        if (GUILayout.Button("선택된 오브젝트 번호 다시 매기기"))
        {
            RenameSelectedObjects();
        }
    }

    void RenameSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length == 0)
        {
            Debug.LogWarning("오브젝트를 선택해주세요.");
            return;
        }

        // 이름순으로 정렬
        System.Array.Sort(selected, (a, b) => a.name.CompareTo(b.name));

        Undo.RecordObjects(selected, "Rename Objects");

        for (int i = 0; i < selected.Length; i++)
        {
            if (i == 0)
                selected[i].name = baseName; // 첫 번째는 괄호 없이
            else
                selected[i].name = $"{baseName} ({i})"; // 두 번째부터 괄호 숫자
        }

        Debug.Log($"{selected.Length}개의 오브젝트 이름이 재정렬되었습니다.");
    }
}
