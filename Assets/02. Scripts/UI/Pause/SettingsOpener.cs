using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsOpener : MonoBehaviour
{
    [Tooltip("지정하지 않으면 같은 Canvas에서 자동 탐색")]
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
        else Debug.LogWarning("설정화면 못찾음");
    }
}
