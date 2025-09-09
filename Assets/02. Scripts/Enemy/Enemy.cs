using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    public float ViewDistance => light2D.pointLightOuterRadius;
    public float ViewAngle => light2D.pointLightInnerAngle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color alertColor;
    private Collider2D coll;
    private Light2D light2D;
    private NavMeshAgent agent;
    private Transform target;
    private StateMachine stateMachine;

    public NavMeshAgent Agent => agent;
    public StateMachine StateMachine => stateMachine;

    public Transform[] patrolPoints;
    public LayerMask TargetMask => targetMask;
    public LayerMask ObstacleMask => obstacleMask;

    public Transform Target => target;
    public bool HasTarget => target;


    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine(this);

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        stateMachine?.UpdateState();

        /*
        FindTarget();

        if (target)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            light2D.color = originalColor;
        }
        */
    }

    public void MoveTo(Vector2 destination)
        => agent.SetDestination(destination);

    public void ChangeState<T>() where T : IState
        => stateMachine?.ChangeState<T>();


    public void FindTarget()
    {
        Transform findTarget = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask);

        if (findTarget)
        {
            ConditionalLogger.Log(findTarget);
            light2D.color = alertColor;
            target = findTarget.transform;
        }
        else
        {
            light2D.color = originalColor;
            target = null;
        }
    }

}
