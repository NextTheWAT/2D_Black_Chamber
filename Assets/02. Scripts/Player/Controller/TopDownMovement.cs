using UnityEngine;

[RequireComponent(typeof(TopDownController), typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    private TopDownController controller;
    private Rigidbody2D rb;
    private CharacterAnimationController animController;

    // 추가: 상태 참조
    private PlayerInputController input;
    private PlayerConditionManager condition;

    public Transform mouseTr;
    public float maxMouseDistance = 5f;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float runSpeedMultiplier = 1.5f; // 필요 시 조절

    [Header("Rotate")]
    [SerializeField] private bool rotateToLook = true;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Animation Blend")]
    [SerializeField] private float walkBlend = 0.5f; // 걷기
    [SerializeField] private float runBlend = 1.0f; // 뛰기

    private Vector2 moveInput;
    private Vector2 curVel;
    private Vector2 lastLookDir = Vector2.up;
    private bool isRunning; // 최종 달리기 상태

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<CharacterAnimationController>();
        input = GetComponent<PlayerInputController>();
        condition = GetComponent<PlayerConditionManager>();

        condition = PlayerConditionManager.Instance;

        rb.gravityScale = 0f;
        rb.freezeRotation = false;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        AudioListener audioListener = FindAnyObjectByType<AudioListener>();

        if(audioListener.transform != this)
            Destroy(audioListener);
    }

    private void OnEnable()
    {
        controller.OnMoveEvent += HandleMove;
        controller.OnLookEvent += HandleLook;
    }

    private void OnDisable()
    {
        controller.OnMoveEvent -= HandleMove;
        controller.OnLookEvent -= HandleLook;
    }

    private void FixedUpdate()
    {
        // 0) 달릴지 결정
        bool wantsToRun = (input != null && input.RunHeld) && moveInput.sqrMagnitude > 0.0001f;
        bool canRun = condition == null ? wantsToRun : (wantsToRun && condition.CanRun);
        isRunning = canRun;

        // 1) 목표 속도
        float speed = moveSpeed * (isRunning ? runSpeedMultiplier : 1f);
        Vector2 targetVel = moveInput.normalized * speed;

        // 2) 가감속
        curVel = Vector2.MoveTowards(curVel, targetVel, acceleration * Time.fixedDeltaTime);

        // 3) 이동
        rb.MovePosition(rb.position + curVel * Time.fixedDeltaTime);

        // 4) 회전(마우스 에임 기준)
        if (rotateToLook && lastLookDir.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(lastLookDir.y, lastLookDir.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        // 5) 스태미나 처리
        if (condition != null)
        {
            if (isRunning && curVel.sqrMagnitude > 0.01f)
                condition.ConsumeForRun(Time.fixedDeltaTime);  // 달리는 중 소모
            else
                condition.TickRegen(Time.fixedDeltaTime);      // 멈춤/걷기 = 재생
        }

        // 6) 애니메이션
        UpdateAnimation();

        // 7) 마우스 미니 포인터(있다면)
        if (mouseTr != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = Vector2.ClampMagnitude(mousePos - (Vector2)transform.position, maxMouseDistance) + (Vector2)transform.position;
            mouseTr.position = mousePos;
        }

        // 8) 무기 반동 처리
        Shooter shooter = WeaponManager.Instance.CurrentWeapon;

        if (shooter && shooter.gunData)
            shooter.CurrentSpread += curVel.magnitude * Time.fixedDeltaTime;
    }

    private void HandleMove(Vector2 v) => moveInput = v;

    private void HandleLook(Vector2 v)
    {
        if (v.sqrMagnitude > 0.0001f)
            lastLookDir = v.normalized;
    }

    private void UpdateAnimation()
    {
        if (animController == null) return;

        // 이동 중이면 0.5(걷기) 또는 1.0(뛰기), 정지면 0
        float tree = isRunning ? runBlend : walkBlend;
        float blend = curVel.sqrMagnitude > 0.01f ? tree : 0f;
        animController.SetMoveBlend(blend);

        // 하체 방향 회전(원 코드 유지)
        animController.SetLowerBodyRotation(moveInput);
    }
}
