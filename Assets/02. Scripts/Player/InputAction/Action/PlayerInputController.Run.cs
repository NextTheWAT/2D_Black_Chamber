using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    [SerializeField] private PlayerConditionManager condition; // 같은 오브젝트에 붙이길 권장
    private bool runHeld;
    public bool RunHeld => runHeld;

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) runHeld = true;
        else if (ctx.phase == InputActionPhase.Canceled) runHeld = false;
        // 달리기 가능 여부 체크는 이동 로직(TopDownMovement) 쪽에서 최종 결정
    }
}
