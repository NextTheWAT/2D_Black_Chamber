using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    [SerializeField] private bool continuousMouseAim = true; // 매 프레임 마우스 위치로 에임 갱신
    private Camera _camera;

    // Look : (선택) 액션으로도 들어오면 그대로 반영 — 하지만 위의 Update가 계속 커버함
    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (_camera == null) return;

        // 스틱 에임 같은 비마우스 입력도 지원
        if (Mouse.current == null)
        {
            Vector2 aim = ctx.ReadValue<Vector2>();
            if (aim.sqrMagnitude > 0.0001f) CallLookEvent(aim.normalized);
        }
        // 마우스일 경우엔 Update에서 매 프레임 처리하므로 여기선 굳이 처리할 필요 없음
    }
}
