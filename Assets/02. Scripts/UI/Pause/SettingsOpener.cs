using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsOpener : MonoBehaviour
{
    [Tooltip("�������� ������ ���� Canvas���� �ڵ� Ž��")]
    [SerializeField] private SettingsOverlay target;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenSettings);
    }

    private void OpenSettings()
    {
        var overlay = target;
        if (overlay == null)
        {
            var canvas = GetComponentInParent<Canvas>(true);
            if (canvas != null)
                overlay = canvas.GetComponentInChildren<SettingsOverlay>(true);
        }

        if (overlay != null) overlay.Open();
        else Debug.LogWarning("����ȭ�� ��ã��");
    }
}
