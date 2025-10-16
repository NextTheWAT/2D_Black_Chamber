using Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using static Item;
using static UnityEditor.Progress;

public class Enemy : MonoBehaviour
{
    [Header("FSM")]
    [SerializeField] private StateTable stateTable;
    [SerializeField] private NonCombatStateType nonCombatStateType;
    [SerializeField] private CombatStateType combatStateType;
    [SerializeField] private bool useCollisionEnter = true;

    [Header("Stat")]
    [SerializeField] private Health health;
    [SerializeField] private float angularSpeed = 120f;

    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask bodyMask;
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


    // === UI: Alert Icons ===
    private enum AlertIconState { None, Suspicious, Alert }

    [Header("UI - Alert Icons")]
    [SerializeField] private GameObject questionIcon;     // ? 오브젝트 (애니메이션 포함 가능)
    [SerializeField] private GameObject exclamationIcon;  // ! 오브젝트 (애니메이션 포함 가능)
    [SerializeField] private float minIconShowTime = 0.12f; // 너무 깜빡임 방지

    private AlertIconState _iconState = AlertIconState.None;
    private float _lastIconChangeTime = -999f;


    public string stateType;
    public bool isTarget = false;

    public GameObject dropItems;
    private Collider2D coll;
    private NavMeshAgent agent;
    private Transform target;
    private StateMachine noncombatStateMachine;
    private StateMachine combatStateMachine;
    private CharacterAnimationController animationController;
    public bool IsHit { get; private set; }
    public bool IsDead => health.IsDead;
    public StateMachine CurrentStateMachine => HasTarget ? combatStateMachine : noncombatStateMachine;
    public StateMachine PreviousStateMachine => previousHasTarget ? combatStateMachine : noncombatStateMachine;

    public float ViewDistance => forwardLight.pointLightOuterRadius;
    public float ViewAngle => forwardLight.pointLightOuterAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public Transform ReturnPoint => returnPoint;

    public Shooter Shooter => shooter;
    public NavMeshAgent Agent => agent;
    public CharacterAnimationController AnimationController => animationController;

    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            if (target)
                LastKnownTargetPos = target.position;

            UpdateLight();
        }
    }
    public bool HasTarget => Target;
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

    private Transform newFoundBody; // 새로 발견한 시체

    private HashSet<Transform> foundBodies; // 발견했던 적 시체들 

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
        get => targetInFOV;
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

    public bool IsNoiseDetected { get; set; } = false; // 소음 감지 여부

    public bool IsBodyDetected
        => newFoundBody;


    private bool previousHasTarget = false;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        foundBodies = new();

        if (isTarget)
        {
            noncombatStateMachine = new TargetFSM(this, stateTable, typeof(PatrolState));
            combatStateMachine = new TargetFSM(this, stateTable, typeof(FleeState));
        }
        else
        {
            noncombatStateMachine = StateMachineFactory.CreateStateMachine(this, stateTable, typeof(PatrolState), nonCombatStateType);
            combatStateMachine = StateMachineFactory.CreateStateMachine(this, stateTable, typeof(CoverState), combatStateType);
        }


        OnPhaseChanged(GameManager.Instance.CurrentPhase); // 현재 페이즈에 맞춰 타겟 설정
        CurrentStateMachine.Start();

        previousHasTarget = HasTarget;

        SetIconState(AlertIconState.None);
        UpdateAlertIcons();

    }

    private void OnEnable()
    {
        if (GameManager.AppIsQuitting) return;
        GameManager.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    void OnDisable()
    {
        if (GameManager.AppIsQuitting) return;
        GameManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }


    private void Update()
    {
        if (previousHasTarget != HasTarget)
        {
            PreviousStateMachine?.Stop();
            CurrentStateMachine?.Start();

            previousHasTarget = HasTarget;
            ConditionalLogger.Log($"Switch State Machine {(HasTarget ? "Combat" : "Non-Combat")}");
        }

        TargetInFOV = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask); // 시야 내 타겟 갱신

        // 새로운 시체 발견시 기록
        if (!GameManager.Instance.IsCombat)
        {
            Transform body = transform.FindTargetInFOV(ViewAngle, ViewDistance, bodyMask, obstacleMask); // 시체 갱신

            if (body && !foundBodies.Contains(body))
            {
                Enemy enemy = body.GetComponent<Enemy>();
                if (enemy && enemy.IsDead)
                {
                    newFoundBody = body;
                    foundBodies.Add(newFoundBody);
                    LastKnownTargetPos = body.position;
                }
            }
        }

        CurrentStateMachine?.UpdateState(); // 상태 머신 업데이트
        stateType = CurrentStateMachine?.CurrentState?.ToString(); // 디버그용 상태 타입 문자열
        UpdateMoveBlend(); // 이동 애니메이션 블렌드값 갱신
        Rotate(); // 회전


        if (IsHit) IsHit = false; // 맞았던 상태 초기화
        if (IsNoiseDetected) IsNoiseDetected = false; // 소음 감지 상태 초기화
        if (IsBodyDetected) newFoundBody = null; // 시체 감지 상태 초기화

        UpdateAlertIcons(); // ?,! 아이콘 상태 갱신
    }

    // ?,! 아이콘 상태 변경
    private void SetIconState(AlertIconState next)
    {
        _iconState = next;
        _lastIconChangeTime = Time.time;

        if (questionIcon) questionIcon.SetActive(next == AlertIconState.Suspicious);
        if (exclamationIcon) exclamationIcon.SetActive(next == AlertIconState.Alert);
    }

    private void UpdateAlertIcons()
    {
        bool isCombat = GameManager.Instance.IsCombat || HasTarget;   // 난전 or 타깃 확정
        bool isSuspicious = !isCombat && HasTargetInFOV;              // 시야엔 들어왔지만 난전 아님

        AlertIconState next = AlertIconState.None;
        if (isCombat) next = AlertIconState.Alert;
        else if (isSuspicious) next = AlertIconState.Suspicious;

        // 너무 자주 깜빡이는 것 방지
        if (next != _iconState && Time.time - _lastIconChangeTime >= minIconShowTime)
            SetIconState(next);
    }
    //여기까지 아이콘

    void OnPhaseChanged(GamePhase phase)
    {
        Target = (phase == GamePhase.Combat) ? GameManager.Instance.Player : null;
        UpdateAlertIcons();   // ← 여기서 즉시 ? / ! 갱신
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
    {
        if (shooter.CurrentMagazine > 0)
        {
            if (shooter.Shoot(transform.up))
                animationController.PlayShoot();
        }
        else
        {
            animationController.PlayReload();
            shooter.Reload();
        }
    }

    public void UpdateLight()
    {
        if (forwardLight == null || backwardLight == null)
        {
            ConditionalLogger.LogWarning($"{gameObject} Enemy Light2D is not assigned.");
            return;
        }

        forwardLight.enabled = true;
        backwardLight.enabled = true;

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

    public void HeardNoise(Vector2 noisePosition)
    {
        if (GameManager.Instance.IsCombat) return;
        IsNoiseDetected = true;
        LastKnownTargetPos = noisePosition;
    }

    public void Hit(int currentHealth, int maxHealth)
    {
        if (currentHealth == maxHealth) return;


        if (GameManager.Instance.IsCombat || HasTarget || HasTargetInFOV)
        {
            animationController.PlayHit();
            LastKnownTargetPos = GameManager.Instance.Player.position;
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
        coll.isTrigger = true;
        enabled = false;
        forwardLight.enabled = false;
        backwardLight.enabled = false;

        GameManager.Instance.CancelCombatDelay(this);

        // 미션 카운팅 감소
        GetComponent<MissionEntityHook>()?.NotifyLogicalDeath();

        questionIcon?.SetActive(false);
        exclamationIcon?.SetActive(false);

       

        Vector3 dropPos = transform.position + Vector3.up * 0.2f;
        GameObject ob = Instantiate(dropItems, dropPos, Quaternion.identity);
        switch(WeaponManager.Instance.CurrentWeaponIndex)
        {
            case 0:
                ob.GetComponent<Item>().ammoAmount = 12;
                break;

            case 1:
                ob.GetComponent<Item>().ammoAmount = 30;
                break;

        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!useCollisionEnter) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Target = collision.transform;
            GameManager.Instance.StartCombatAfterDelay(this);
        }
        else if (collision.gameObject.CompareTag("Door"))
        {
            Door door = collision.gameObject.GetComponentInParent<Door>();
            if (door && !door.isExitDoor && !door.IsOpen)
            {
                door.Interaction(transform);
            }

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
