using UnityEngine;

[RequireComponent(typeof(TopDownController), typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    private TopDownController controller;
    private Rigidbody2D rb;
    private CharacterAnimationController animController;
    public Transform mouseTr;
    public float maxMouseDistance = 5f;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;      // 유닛/초
    [SerializeField] private float acceleration = 30f;  // 가감속

    [Header("Rotate")]
    [SerializeField] private bool rotateToLook = true;   // 마우스 에임 방향 회전
    [SerializeField] private float rotationSpeed = 720f; // deg/sec

    [Header("Animation (0.5 - Walk) (1 - Run)")]
    [SerializeField] private float animationTree = 0.5f; // 0.5 - 걷기, 1 - 뛰기

    private Vector2 moveInput;     // WASD (이동 전용)
    private Vector2 curVel;        // 현재 속도(유닛/초)
    private Vector2 lastLookDir = Vector2.up; // 항상 이 방향을 바라봄(마우스 기준)

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<CharacterAnimationController>();

        // rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = false; // 스크립트로 회전
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
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
        // 1) 목표 속도
        Vector2 targetVel = moveInput.normalized * moveSpeed;

        // 2) 가감속
        curVel = Vector2.MoveTowards(curVel, targetVel, acceleration * Time.fixedDeltaTime);

        // 3) Kinematic 이동
        rb.MovePosition(rb.position + curVel * Time.fixedDeltaTime);

        // 4) 회전: 무조건 마우스 에임(lastLookDir)만 기준
        if (rotateToLook && lastLookDir.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(lastLookDir.y, lastLookDir.x) * Mathf.Rad2Deg - 90f; // 위(+Y) 기준 스프라이트
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        // 5) 애니메이션
        UpdateAnimation();

        // 6) 마우스 위치 갱신
        if (mouseTr != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = Vector2.ClampMagnitude(mousePos - (Vector2)transform.position, maxMouseDistance) + (Vector2)transform.position;
            mouseTr.position = mousePos;
        }
    }

    private void HandleMove(Vector2 v) => moveInput = v;

    // 마우스 에임 갱신(정규화 & 0 입력 무시)
    private void HandleLook(Vector2 v)
    {
        if (v.sqrMagnitude > 0.0001f)
            lastLookDir = v.normalized;
    }

    private void UpdateAnimation()
    {
        if (animController == null) return;

        float blend = curVel.sqrMagnitude > 0.01f ? animationTree : 0f; // 0~1
        animController.SetMoveBlend(blend);

        // 이동방향으로 하체 회전
        animController.SetLowerBodyRotation(moveInput);
    }
}
