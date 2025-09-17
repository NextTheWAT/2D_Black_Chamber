using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using System;
using Random = UnityEngine.Random;
using System.Collections;

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
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] deathSounds;

    [Header("Return")]
    [SerializeField] private Transform returnPoint;

    private Collider2D coll;
    private NavMeshAgent agent;
    private Transform target;
    private Vector2 lastKnownTargetPos;
    private StateMachine stateMachine;
    private CharacterAnimationController animationController;
    private AudioSource audioSource;
    public bool IsHit { get; private set; }

    public float ViewDistance => forwardLight.pointLightOuterRadius;
    public float ViewAngle => forwardLight.pointLightOuterAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public NavMeshAgent Agent => agent;
    public CharacterAnimationController AnimationController => animationController;

    private bool hasSuspiciousTarget = false;

    public bool HasSuspiciousTarget
    {
        get => hasSuspiciousTarget;
        set
        {
            hasSuspiciousTarget = value;
            UpdateLight();
        }
    }
    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            if (target)
            {
                LastKnownTargetPos = target.position;
            }


            UpdateLight();
        }
    }
    public bool HasTarget => target;
    public Vector2 LastKnownTargetPos
    {
        get => lastKnownTargetPos;
        set => lastKnownTargetPos = value;
    }

    public bool IsTargetInSight
    {
        get
        {
            if (!HasTarget) return false;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, Target.position, targetMask | obstacleMask);
            return hit.collider != null && hit.collider.CompareTag("Player");
        }
    }

    public Health Health => health;

    public string stateType;

    public bool isTarget = false;
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

    public Transform ReturnPoint;


    private void Start()
    {
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        audioSource = GetComponent<AudioSource>();

        backwardLight.color = alertColor;

        if (isTarget)
            stateMachine = new TargetFSM(this, stateTable);
        else
            stateMachine = new SoliderFSM(this, stateTable);
    }

    private void Update()
    {
        stateMachine?.UpdateState();
        UpdateMoveBlend();
        stateType = stateMachine?.CurrentState.ToString();

        if(IsHit)
            IsHit = false;
    }

    private void UpdateMoveBlend()
    {
        float moveBlend = agent.velocity.magnitude > 0.01f ? 0.5f : 0f;
        animationController.SetMoveBlend(moveBlend);
    }

    public void MoveTo(Vector2 destination){
        if ((Vector3)destination == agent.destination) return;
        agent.SetDestination(destination);
    }

    public void RotateTo(float targetAngle)
    {
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * angularSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void LookAt(Vector2 lookPoint)
    {
        Vector2 direction = (lookPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        RotateTo(angle);
    }

    public void Attack()
    {
        shooter.Shoot(transform.up);
    }

    public void UpdateLight()
    {
        forwardLight.enabled = HasTarget || HasSuspiciousTarget;
        backwardLight.enabled = HasTarget || HasSuspiciousTarget;

        forwardLight.color = originalColor;
        backwardLight.color = originalColor;

        if (HasSuspiciousTarget)
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

    public Transform FindSuspiciousTarget()
    {
        Transform target = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask);
        HasSuspiciousTarget = target;
        LastKnownTargetPos = target ? target.position : LastKnownTargetPos;
        return target;
    }

    public void FindTarget(float viewAngle, float viewDistance)
    {
        Target = transform.FindTargetInFOV(viewAngle, viewDistance, targetMask, obstacleMask);
    }
    public void FindTarget()
    {
        Target = transform.FindTargetInFOV(ViewAngle, ViewDistance, targetMask, obstacleMask);
    }


    public void Hit(int currentHealth, int maxHealth)
    {
        if (currentHealth == maxHealth) return;

        if (GameManager.Instance.IsCombat || hasSuspiciousTarget)
        {
            animationController.PlayHit();
            LastKnownTargetPos = GameManager.Instance.player.position;
            IsHit = true;

            Debug.Log(GameManager.Instance.player);

            if (hitSounds.Length > 0)
            {
                int index = Random.Range(0, hitSounds.Length);
                audioSource.PlayOneShot(hitSounds[index]);
            }
        }
        else
        {
            health.TakeDamage(maxHealth);
        }

    }

    public void Die()
    {
        if (deathSounds.Length > 0)
        {
            int index = Random.Range(0, deathSounds.Length);
            audioSource.PlayOneShot(deathSounds[index]);
        }

        animationController.PlayDie();
        coll.enabled = false;
        agent.isStopped = true;
        enabled = false;
        angularSpeed = 0f;
        Destroy(gameObject, 2f);
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
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
        
    }

}
