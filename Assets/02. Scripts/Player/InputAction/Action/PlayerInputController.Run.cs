using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    [SerializeField] private PlayerConditionManager condition; // ���� ������Ʈ�� ���̱� ����
    private bool runHeld;
    public bool RunHeld => runHeld;

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) runHeld = true;
        else if (ctx.phase == InputActionPhase.Canceled) runHeld = false;
        // �޸��� ���� ���� üũ�� �̵� ����(TopDownMovement) �ʿ��� ���� ����
    }
}
