using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputController : TopDownController
{
    [SerializeField] Shooter shooter;
    private CharacterAnimationController animationController;

    private void Awake()
    {
        shooter = GetComponent<Shooter>();
        _camera = Camera.main;
        animationController = GetComponent<CharacterAnimationController>();

        WeaponManager.Instance.OnWeaponChanged.AddListener((shooter) =>
        {
            if (shooter == null || shooter.gunData == null) return;

            this.shooter = shooter;
            animationController.ApplyUpperWeaponAnimator(shooter.gunData, playSwitchAnim: true);
        });
    }

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

            if (shootPressed)
                GunForward();
        }
    }

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
