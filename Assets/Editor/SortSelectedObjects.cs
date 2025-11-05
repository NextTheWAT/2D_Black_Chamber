using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class SortSelectedObjectsByNameAndNumber : EditorWindow
{
    [MenuItem("Tools/Sort Selected Objects by Name + Number")]
    static void SortSelectedObjects()
    {
        var selected = Selection.transforms;

        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("오브젝트를 하나 이상 선택하세요!");
            return;
        }

        // 부모 기준으로 그룹화
        var parentGroups = selected.GroupBy(t => t.parent);

        int total = 0;
        foreach (var group in parentGroups)
        {
            // 이름 + 숫자 순서대로 정렬
            var ordered = group
                .Select(t => new
                {
                    transform = t,
                    baseName = GetBaseName(t.name),
                    hasNumber = HasNumber(t.name),
                    number = ExtractNumber(t.name)
                })
                .OrderBy(x => x.baseName, System.StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.hasNumber) // 괄호 없는 게 먼저 오게 (false < true)
                .ThenBy(x => x.number)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].transform.SetSiblingIndex(i);
                total++;
            }
        }

        Debug.Log($"✅ 선택한 {total}개의 오브젝트를 이름 + 숫자 순으로 정렬 완료!");
    }

    // 이름에서 괄호 안 숫자 추출
    static int ExtractNumber(string name)
    {
        var match = Regex.Match(name, @"\((\d+)\)$");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
            return num;
        return int.MaxValue; // 숫자 없는 경우 뒤로
    }

    // 괄호 안 숫자 존재 여부
    static bool HasNumber(string name)
    {
        return Regex.IsMatch(name, @"\(\d+\)$");
    }

    // 이름에서 "(숫자)" 제거한 기본 이름
    static string GetBaseName(string name)
    {
        return Regex.Replace(name, @"\s*\(\d+\)\s*$", "").Trim();
    }
}
