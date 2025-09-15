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
            UpdateLightColor();
        }
    }
    public Transform Target
    {
        get => target;
        set
        {
            target = value;
            if (target)
                LastKnownTargetPos = target.position;

            UpdateLightColor();
        }
    }
    public bool HasTarget => target;
    public Vector2 LastKnownTargetPos
    {
        get => lastKnownTargetPos;
        set => lastKnownTargetPos = value;
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


    private void Start()
    {
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<CharacterAnimationController>();
        audioSource = GetComponent<AudioSource>();

        backwardLight.color = alertColor;


        if(isTarget)
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
        float moveBlend = agent.velocity.magnitude > 0.01f ? 1f : 0f;
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

    public void UpdateLightColor()
    {
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

        animationController.PlayHit();
        LastKnownTargetPos = GameManager.Instance.player.position;
        IsHit = true;

        if (hitSounds.Length > 0)
        {
            int index = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[index]);
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
        Destroy(gameObject, 2f);
    }

    private void OnBecameVisible()
    {
        forwardLight.enabled = true;
    }

    private void OnBecameInvisible()
    {
        forwardLight.enabled = false;
    }

}
