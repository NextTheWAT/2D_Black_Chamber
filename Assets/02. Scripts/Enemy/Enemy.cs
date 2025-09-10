using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
public enum EnemyState
{
    Patrol,
    Chase,
    Investigate,
    Return,
    Attack
}

public enum PatrolType
{
    Waypoint,
    Fixed
}

public class Enemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color alertColor;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int startPatrolPointIndex;
    [SerializeField] private PatrolType patrolType;

    private Collider2D coll;
    private Light2D light2D;
    private NavMeshAgent agent;
    private Transform target;
    private StateMachine stateMachine;
    private AgentRotateSmooth2d agentRotate;

    public float ViewDistance => light2D.pointLightOuterRadius;
    public float ViewAngle => light2D.pointLightInnerAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public PatrolType PatrolType => patrolType;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public NavMeshAgent Agent => agent;
    public StateMachine StateMachine => stateMachine;

    public LayerMask TargetMask => targetMask;
    public LayerMask ObstacleMask => obstacleMask;

    public Transform Target => target;
    public bool HasTarget => target;

    public float RotateSpeed => agentRotate.angularSpeed;

    public float StopDistance
    {
        get => Agent.stoppingDistance;
        set
        {
            Agent.stoppingDistance = value;
        }
    }


    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine(this);
        agentRotate = GetComponent<AgentRotateSmooth2d>();

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
    }

    public void MoveTo(Vector2 destination)
        => agent.SetDestination(destination);

    public void RotateTo(float targetAngle)
    {
        if (agentRotate == null) return;

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


    public void FindTarget()
    {
        Transform findTarget = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask);

        if (findTarget)
        {
            light2D.color = alertColor;
            target = findTarget.transform;
        }
        else
        {
            light2D.color = originalColor;
            target = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collsion {collision.transform}");
        if (collision.transform == target)
        {
            Debug.Log($"Attack {target}");
            Destroy(collision.gameObject);
            ChangeState<ReturnState>();
        }
    }

}
