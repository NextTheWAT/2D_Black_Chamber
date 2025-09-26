using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    public void OnMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // 1) 설정 팝업이 떠 있으면 닫기
        var setting = Object.FindFirstObjectByType<SettingPopup>(FindObjectsInactive.Include);
        if (setting != null && setting.gameObject.activeInHierarchy)
        {
            setting.RequestClose();
            return;
        }

        // 2) 일시정지 
        var pause = Object.FindFirstObjectByType<PausePopup>(FindObjectsInactive.Include);
        if (pause != null)
        {
            if (pause.gameObject.activeInHierarchy)
                pause.RequestClose();
            else
                UIManager.Instance.OpenUI<PausePopup>();
        }
    }
}
