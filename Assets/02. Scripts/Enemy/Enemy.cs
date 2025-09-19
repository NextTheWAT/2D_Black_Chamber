using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private Health health;
    [SerializeField] private StateTable stateTable;
    [SerializeField] private float angularSpeed = 120f;

    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Light2D forwardLight;
    [SerializeField] private Light2D backwardLight;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color suspiciousColor;
    [SerializeField] private Color alertColor;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int startPatrolPointIndex;

    [Header("Attack")]
    [SerializeField] private Shooter shooter;

    [Header("Return")]
    [SerializeField] private Transform returnPoint;

    public string stateType;
    public bool isTarget = false;

    private Collider2D coll;
    private NavMeshAgent agent;
    private Transform target;
    private StateMachine stateMachine;
    private CharacterAnimationController animationController;
    public bool IsHit { get; private set; }
    public bool IsDead => health.IsDead;

    public float ViewDistance => forwardLight.pointLightOuterRadius;
    public float ViewAngle => forwardLight.pointLightOuterAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public Transform ReturnPoint => returnPoint;

    public NavMeshAgent Agent => agent;
    public CharacterAnimationController AnimationController => animationController;

    public Transform Target
    {
        get
        {
            if (GameManager.Instance.IsCombat) return GameManager.Instance.player;
            return target;
        }
        set
        {
            target = value;
            if (target)
                LastKnownTargetPos = target.position;

            UpdateLight();
        }
    }
    public bool HasTarget => target;
    public Vector2 LastKnownTargetPos { get; set; }

    public bool IsTargetInSight
    {
        get
        {
            if (!HasTarget) return false;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, Target.position, targetMask | obstacleMask);
            return hit.collider != null && hit.collider.CompareTag("Player");
        }
    }
    private bool nearbyDeathTriggered;
    public bool NearbyDeathTriggered
    {
        get
        {
            if (nearbyDeathTriggered)
            {
                nearbyDeathTriggered = false; // 읽는 순간 소비
                return true;
            }
            return nearbyDeathTriggered;
        }
        set => nearbyDeathTriggered = value;
    }

    public Health Health => health;

    // 목적지에 도착했는지
    public bool IsArrived
    {
        get
        {
            if (!Agent.pathPending)
            {
                if (Agent.remainingDistance <= Agent.stoppingDistance)
                {
                    if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                        return true;
                }
            }
            return false;
        }
    }

    public float CurrentLookAngleDelta // 현재 바라보는 각도와 목표 각도의 차이
    {
        get
        {
            Vector2 direction = (LookPoint - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; // 위쪽을 기준으로 보정
            return Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);
        }
    }

    public Vector2 NextMovePoint
        => Agent.path.corners.Length > 1 ? (Vector2)Agent.path.corners[1] : (Vector2)transform.position;

    public Vector2 MoveDirection
        => Agent.velocity.normalized;

    private Transform targetInFOV;
    public Transform TargetInFOV
    {
        get { return targetInFOV; }
        set
        {
            targetInFOV = value;
            if (targetInFOV)
                LastKnownTargetPos = targetInFOV.position;
            UpdateLight();
        }
    }
    public bool HasTargetInFOV
        => TargetInFOV;

    public Vector2 LookPoint { get; set; } = Vector2.zero;

    public bool AutoRotate { get; set; }

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();

        if (isTarget)
            stateMachine = new TargetFSM(this, stateTable);
        else
            stateMachine = new SoliderFSM(this, stateTable);
    }

    private void Update()
    {
        TargetInFOV = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask); // 시야 내 타겟 갱신
        stateMachine?.UpdateState(); // 상태 머신 업데이트
        stateType = stateMachine?.CurrentState.ToString(); // 디버그용 상태 타입 문자열
        UpdateMoveBlend(); // 이동 애니메이션 블렌드값 갱신
        Rotate(); // 회전

        // 맞았던 상태 초기화
        if(IsHit)
            IsHit = false;
    }

    private void UpdateMoveBlend()
    {
        float moveBlend = agent.velocity.magnitude > 0.01f ? 0.5f : 0f;
        animationController.SetMoveBlend(moveBlend);
    }

    public void MoveTo(Vector2 destination, Vector2 lookPoint)
    {
        if ((Vector3)destination == agent.destination) return;
        agent.SetDestination(destination);
        LookPoint = lookPoint;
    }

    public void MoveTo(Vector2 destination)
        => MoveTo(destination, NextMovePoint);

    void Rotate()
    {
        if (AutoRotate)
        {
            if (!IsArrived)
                LookPoint = NextMovePoint;
            else
                LookPoint = transform.position + transform.up;
        }

        Vector2 direction = (LookPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; // 위쪽을 기준으로 보정
        angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, Time.deltaTime * angularSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void Attack()
        => shooter.Shoot(transform.up);

    public void UpdateLight()
    {
        forwardLight.enabled = HasTarget || HasTargetInFOV;
        backwardLight.enabled = HasTarget || HasTargetInFOV;

        forwardLight.color = originalColor;
        backwardLight.color = originalColor;

        if (HasTargetInFOV)
        {
            forwardLight.color = suspiciousColor;
            backwardLight.color = alertColor;
        }

        if (HasTarget)
        {
            forwardLight.color = alertColor;
            backwardLight.color = alertColor;
        }
    }

    public void SetBackwardLightRadius(float ratio)
        => backwardLight.pointLightOuterRadius = ViewDistance * ratio;

    public void FindTarget(float viewAngle, float viewDistance)
        => Target = transform.FindTargetInFOV(viewAngle, viewDistance, targetMask, obstacleMask);

    public void Hit(int currentHealth, int maxHealth)
    {
        if (currentHealth == maxHealth) return;


        if (GameManager.Instance.IsCombat || HasTargetInFOV)
        {
            animationController.PlayHit();
            LastKnownTargetPos = GameManager.Instance.player.position;
            IsHit = true;
            CharacterSoundManager.Instance.PlayHitSound();
        }
        else
            health.TakeDamage(maxHealth);
    }

    public void Die()
    {
        CharacterSoundManager.Instance.PlayDieSound();

        agent.enabled = false;
        animationController.PlayDie();
        coll.enabled = false;
        enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Target = collision.transform;
            GameManager.Instance.IsCombat = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.green;
            var path = agent.path;
            var corners = path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                Gizmos.DrawLine(corners[i], corners[i + 1]);

            for (int i = 0; i < corners.Length; i++)
                Gizmos.DrawSphere(corners[i], 0.2f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(LookPoint, 0.2f);
    }

}
