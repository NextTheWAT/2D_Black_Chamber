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

    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        FindTarget();


        if (target)
        {
            agent.SetDestination(target.position);
        }
    }

    void FindTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, ViewDistance, targetMask);
        Transform findTarget = null;

        foreach (Collider2D col in targets)
        {
            if (col == coll) continue;

            if (col is CircleCollider2D circle)
            {
                if (transform.IsCircleVisible(circle, ViewAngle, ViewDistance, targetMask, obstacleMask))
                {
                    ConditionalLogger.Log(col);
                    light2D.color = alertColor;
                    findTarget = col.transform;
                }
                else
                {
                    light2D.color = originalColor;
                }
            }
        }
        target = findTarget;
    }

}
