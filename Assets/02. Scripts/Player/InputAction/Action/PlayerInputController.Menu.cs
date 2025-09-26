using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    public void OnMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        // 1) ���� �˾��� �� ������ �ݱ�
        var setting = Object.FindFirstObjectByType<SettingPopup>(FindObjectsInactive.Include);
        if (setting != null && setting.gameObject.activeInHierarchy)
        {
            setting.RequestClose();
            return;
        }

        // 2) �Ͻ����� 
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
