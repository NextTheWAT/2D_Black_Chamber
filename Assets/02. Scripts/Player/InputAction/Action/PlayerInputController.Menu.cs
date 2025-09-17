using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    public void OnMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return; // 키를 눌렀을 때만 반응

        var pauseOverlay = FindObjectOfType<UIPauseOverlay>(true);
        if (pauseOverlay != null)
        {
            pauseOverlay.TogglePause(); // ESC 누르면 일시정지 화면 띄우기
        }
        else
        {
            Debug.LogWarning("UIPauseOverlay를 찾을 수 없습니다!");
        }
    }
}
