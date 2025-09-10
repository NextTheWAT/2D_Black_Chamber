using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{

    private void LateUpdate()
    {
        // 마우스가 있으면 매 프레임 에임 갱신(카메라/플레이어 이동에도 정확)
        if (continuousMouseAim && _camera != null && Mouse.current != null)
        {
            Vector2 screen = Mouse.current.position.ReadValue();
            float zDist = Mathf.Abs(_camera.transform.position.z - transform.position.z);
            Vector3 world = _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, zDist));
            Vector2 aim = (Vector2)(world - transform.position);
            if (aim.sqrMagnitude > 0.0001f) CallLookEvent(aim.normalized);
        }
    }

}
