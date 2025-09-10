using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    //[SerializeField] private CrosshairUI crosshair; // �ν����Ϳ��� �Ҵ�

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        //crosshair?.Pulse();   // ũ�ν���� �޽�

        UICrosshairManager.Instance.crosshairUI.Pulse(); // �̱������� �����Ͽ� �޽� ȣ��

        // ���� ������ ���߿� �߰�
    }
}
