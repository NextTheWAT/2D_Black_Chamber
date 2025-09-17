using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    public void OnMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return; // Ű�� ������ ���� ����

        var pauseOverlay = FindObjectOfType<UIPauseOverlay>(true);
        if (pauseOverlay != null)
        {
            pauseOverlay.TogglePause(); // ESC ������ �Ͻ����� ȭ�� ����
        }
        else
        {
            Debug.LogWarning("UIPauseOverlay�� ã�� �� �����ϴ�!");
        }
    }
}
