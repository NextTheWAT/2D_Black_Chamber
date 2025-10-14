using TMPro;
using UnityEngine;

public class UIMissionGoalText : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = Color.white; // ������
    [SerializeField] private Color completedColor = new(0.25f, 0.8f, 0.45f); //�Ϸ� �� �ʷϻ�

    private void Awake()
    {
        if (!label) label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        var mm = MissionManager.Instance;
        if (mm != null) mm.OnTargetsChanged += Refresh;
        Refresh(mm != null ? mm.RemainingEnemies : 0);
    }

    private void OnDisable()
    {
        var mm = MissionManager.Instance;
        if (mm != null) mm.OnTargetsChanged -= Refresh;
    }

    private void Refresh(int _remaining)
    {
        var mm = MissionManager.Instance;
        int total = mm != null ? mm.TotalTargets : 0;
        int done = mm != null ? mm.CompletedTargets : 0;

        if (!label) return;
        label.text = $"���� ��� �� {done} / {total}";
        label.color = (total > 0 && done >= total) ? completedColor : normalColor;
    }
}
