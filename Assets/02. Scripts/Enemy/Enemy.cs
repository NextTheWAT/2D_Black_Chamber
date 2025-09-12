using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Constants;

public class Enemy : MonoBehaviour
{
    [Header("Agent")]
    [SerializeField] private float angularSpeed = 120f;

    [Header("Stat")]
    [SerializeField] private Health health;
    [SerializeField] private StateTable stateTable;

    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color alertColor;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int startPatrolPointIndex;
    [SerializeField] private PatrolType patrolType;

    [Header("Chase")]
    [SerializeField] private float initialChaseDelay = 2f; // 처음 타겟 발견했을 때 정지 시간
    [SerializeField] private float investigateThreshold = 3f; // 몇 초 이상 못 보면 조사 상태로 전환
    public float chaseToInvestigateTimer;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Shooter shooter;

    [Header("Flee")]
    [SerializeField] private float fleeDistance = 10f;
    [SerializeField] private float fleeDuration = 5f;
    public float fleeTimer;

    [Header("Investigate")]
    public float investigateDuration = 5f;
    public float investigateRange = 2f;
    public float investigateTimer;


    private Collider2D coll;
    private Light2D light2D;
    private NavMeshAgent agent;
    private Transform target;
    private Vector2 lastKnownTargetPos;
    private StateMachine stateMachine;
    private CharacterAnimationController animationController; 

    public float InitialChaseDelay => initialChaseDelay;
    public float InvestigateThreshold => investigateThreshold;

    public float ViewDistance => light2D.pointLightOuterRadius;
    public float ViewAngle => light2D.pointLightOuterAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public PatrolType PatrolType => patrolType;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public NavMeshAgent Agent => agent;
    public StateMachine StateMachine => stateMachine;
    public CharacterAnimationController AnimationController => animationController;

    public LayerMask TargetMask => targetMask;
    public LayerMask ObstacleMask => obstacleMask;

    public Transform Target => target;
    public Vector2 LastKnownTargetPos
    {
        get => lastKnownTargetPos;
        set => lastKnownTargetPos = value;
    }

    public bool HasTarget => target;

    public float RotateSpeed => angularSpeed;

    public Health Health => health;

    public float StopDistance
    {
        get => Agent.stoppingDistance;
        set
        {
            Agent.stoppingDistance = value;
        }
    }

    public float AttackRange => attackRange;

    public string stateType;

    public bool AgentEnabled
    {
        get => agent.enabled;
        set => agent.enabled = value;
    }

    public float FleeDistance => fleeDistance;
    public float FleeDuration => fleeDuration;

    public bool isTarget = false;

    public bool IsFleeStop => fleeTimer >= fleeDuration;

    public bool IsChaseToInvestigate => chaseToInvestigateTimer >= investigateThreshold;

    public bool IsInvestigateStop => investigateTimer >= investigateDuration;

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

    public bool IsTargetInAttackRange
    {
        get
        {
            if (!HasTarget) return false;
            float distToTarget = Vector2.Distance(transform.position, Target.position);
            return distToTarget <= AttackRange;
        }
    }


    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        if(isTarget)
            stateMachine = new TargetFSM(this, stateTable);
        else
            stateMachine = new SoliderFSM(this, stateTable);
    }

    private void OnEnable()
    {
        stateMachine?.ChangeState(StateType.Patrol);
    }

    private void Update()
    {
        stateMachine?.UpdateState();
        UpdateMoveBlend();
        stateType = stateMachine?.CurrentState.StateType.ToString();
    }

    private void UpdateMoveBlend()
    {
        float moveBlend = agent.velocity.magnitude > 0.01f ? 1f : 0f;
        animationController.SetMoveBlend(moveBlend);
    }

    public void MoveTo(Vector2 destination){
        if (!AgentEnabled) return;
        if ((Vector3)destination == agent.destination) return;
        agent.SetDestination(destination);
    }

    public void RotateTo(float targetAngle)
    {
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * RotateSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void LookAt(Vector2 lookPoint)
    {
        Vector2 direction = (lookPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        RotateTo(angle);
    }

    public void ChangeState(StateType stateType)
        => stateMachine?.ChangeState(stateType);

    public void Attack()
    {
        shooter.Shoot(transform.up);
    }


    public void FindTarget()
    {
        Transform findTarget = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask);

        if (findTarget)
        {
            light2D.color = alertColor;
            target = findTarget.transform;
            lastKnownTargetPos = target.position;
        }
        else
        {
            light2D.color = originalColor;
            target = null;
        }
    }


    public void Hit()
    {
        animationController.PlayHit();
        ChangeState(StateType.Attack);
    }

    public void Die()
    {
        animationController.PlayDie();
        coll.enabled = false;
        agent.isStopped = true;
        enabled = false;
        Destroy(gameObject, 2f);
    }

}
