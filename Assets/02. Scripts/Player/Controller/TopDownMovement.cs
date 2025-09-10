using UnityEngine;

[RequireComponent(typeof(TopDownController), typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    private TopDownController controller;
    private Rigidbody2D rb;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;      // ����/��
    [SerializeField] private float acceleration = 30f;  // ������

    [Header("Rotate")]
    [SerializeField] private bool rotateToLook = true;   // ���콺 ���� ���� ȸ��
    [SerializeField] private float rotationSpeed = 720f; // deg/sec

    private Vector2 moveInput;     // WASD (�̵� ����)
    private Vector2 curVel;        // ���� �ӵ�(����/��)
    private Vector2 lastLookDir = Vector2.up; // �׻� �� ������ �ٶ�(���콺 ����)

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = false; // ��ũ��Ʈ�� ȸ��
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
        // 1) ��ǥ �ӵ�
        Vector2 targetVel = moveInput.normalized * moveSpeed;

        // 2) ������
        curVel = Vector2.MoveTowards(curVel, targetVel, acceleration * Time.fixedDeltaTime);

        // 3) Kinematic �̵�
        rb.MovePosition(rb.position + curVel * Time.fixedDeltaTime);

        // 4) ȸ��: ������ ���콺 ����(lastLookDir)�� ����
        if (rotateToLook && lastLookDir.sqrMagnitude > 0.0001f)
        {
            float targetAngle = Mathf.Atan2(lastLookDir.y, lastLookDir.x) * Mathf.Rad2Deg - 90f; // ��(+Y) ���� ��������Ʈ
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }
    }

    private void HandleMove(Vector2 v) => moveInput = v;

    // ���콺 ���� ����(����ȭ & 0 �Է� ����)
    private void HandleLook(Vector2 v)
    {
        if (v.sqrMagnitude > 0.0001f)
            lastLookDir = v.normalized;
    }
}
