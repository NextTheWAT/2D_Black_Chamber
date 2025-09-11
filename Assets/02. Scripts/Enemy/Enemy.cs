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

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Shooter shooter;

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



    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        stateMachine = new StateMachine(this);

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        stateMachine?.ChangeState<PatrolState>();
    }

    private void Update()
    {
        stateMachine?.UpdateState();
        UpdateMoveBlend();
    }

    private void UpdateMoveBlend()
    {
        float moveBlend = agent.velocity.magnitude > 0.01f ? 1f : 0f;
        animationController.SetMoveBlend(moveBlend);
    }

    public void MoveTo(Vector2 destination)
        => agent.SetDestination(destination);

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

    public void ChangeState<T>() where T : IState
        => stateMachine?.ChangeState<T>();

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
        ChangeState<AttackState>();
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
