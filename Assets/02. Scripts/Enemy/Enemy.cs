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
    public bool IsDead => health.IsDead;

    public float ViewDistance => forwardLight.pointLightOuterRadius;
    public float ViewAngle => forwardLight.pointLightOuterAngle;

    public Transform[] PatrolPoints => patrolPoints;
    public int StartPatrolPointIndex => startPatrolPointIndex;

    public Transform ReturnPoint => returnPoint;

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


    private void Start()
    {
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        audioSource = GetComponent<AudioSource>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (isTarget)
            stateMachine = new TargetFSM(this, stateTable);
        else
            stateMachine = new SoliderFSM(this, stateTable);
    }

    private void Update()
    {
        stateMachine?.UpdateState();
        stateType = stateMachine?.CurrentState.ToString();
        UpdateMoveBlend();
        Rotate();

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
        LookPoint = NextMovePoint;
    }

    public void MoveTo(Vector2 destination)
        => MoveTo(destination, NextMovePoint);

    public Vector2 LookPoint { get; set; } = Vector2.zero;

    void Rotate()
    {
        Vector2 direction = (LookPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90; // 위쪽을 기준으로 보정
        angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, Time.deltaTime * angularSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    /*
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
    */
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
        ConditionalLogger.Log($"{gameObject.name} 사망");
        StopAllCoroutines();

        if (deathSounds.Length > 0)
        {
            int index = Random.Range(0, deathSounds.Length);
            audioSource.PlayOneShot(deathSounds[index]);
        }

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
