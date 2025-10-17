using UnityEngine;

[RequireComponent(typeof(TopDownController), typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    private TopDownController controller;
    private Rigidbody2D rb;
    private CharacterAnimationController animController;

    // �߰�: ���� ����
    private PlayerInputController input;
    private PlayerConditionManager condition;

    public Transform mouseTr;
    public float maxMouseDistance = 5f;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float runSpeedMultiplier = 1.5f; // �ʿ� �� ����

    [Header("Rotate")]
    [SerializeField] private bool rotateToLook = true;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Animation Blend")]
    [SerializeField] private float walkBlend = 0.5f; // �ȱ�
    [SerializeField] private float runBlend = 1.0f; // �ٱ�

    private Vector2 moveInput;
    private Vector2 curVel;
    private Vector2 lastLookDir = Vector2.up;
    private bool isRunning; // ���� �޸��� ����

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
        // 0) �޸��� ����
        bool wantsToRun = (input != null && input.RunHeld) && moveInput.sqrMagnitude > 0.0001f;
        bool canRun = condition == null ? wantsToRun : (wantsToRun && condition.CanRun);
        isRunning = canRun;

        // 1) ��ǥ �ӵ�
        float speed = moveSpeed * (isRunning ? runSpeedMultiplier : 1f);
        Vector2 targetVel = moveInput.normalized * speed;

        // 2) ������
        curVel = Vector2.MoveTowards(curVel, targetVel, acceleration * Time.fixedDeltaTime);

        // 3) �̵�
        rb.MovePosition(rb.position + curVel * Time.fixedDeltaTime);

        // 4) ȸ��(���콺 ���� ����)
        if (rotateToLook && lastLookDir.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(lastLookDir.y, lastLookDir.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        // 5) ���¹̳� ó��
        if (condition != null)
        {
            if (isRunning && curVel.sqrMagnitude > 0.01f)
                condition.ConsumeForRun(Time.fixedDeltaTime);  // �޸��� �� �Ҹ�
            else
                condition.TickRegen(Time.fixedDeltaTime);      // ����/�ȱ� = ���
        }

        // 6) �ִϸ��̼�
        UpdateAnimation();

        // 7) ���콺 �̴� ������(�ִٸ�)
        if (mouseTr != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = Vector2.ClampMagnitude(mousePos - (Vector2)transform.position, maxMouseDistance) + (Vector2)transform.position;
            mouseTr.position = mousePos;
        }

        // 8) ���� �ݵ� ó��
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

        // �̵� ���̸� 0.5(�ȱ�) �Ǵ� 1.0(�ٱ�), ������ 0
        float tree = isRunning ? runBlend : walkBlend;
        float blend = curVel.sqrMagnitude > 0.01f ? tree : 0f;
        animController.SetMoveBlend(blend);

        // ��ü ���� ȸ��(�� �ڵ� ����)
        animController.SetLowerBodyRotation(moveInput);
    }
}
