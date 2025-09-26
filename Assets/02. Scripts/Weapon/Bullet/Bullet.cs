using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Hit Filter")]
    [SerializeField] private LayerMask damageLayers = ~0; // 기본: 전부 허용
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private GameObject damageHitEffect; // 적 충돌 이펙트 프리팹
    [SerializeField] private GameObject obstacleHitEffect; // 장애물 충돌 이펙트 프리팹

    private int dmg;
    private float life;
    private float spawnTime;
    private int ignoreLayer;
    private Vector2 previousPos;
    private float noiseRange;
    private ContactFilter2D damageFilter; // 데미지 충돌 필터
    private ContactFilter2D obstacleFilter; // 장애물 충돌 필터

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // 간단하게 Trigger 충돌만

        // 트리거는 충돌 안하도록 필터 설정
        damageFilter = new();
        damageFilter.SetLayerMask(damageLayers);
        damageFilter.useTriggers = false;

        obstacleFilter = new();
        obstacleFilter.SetLayerMask(obstacleLayers);
        obstacleFilter.useTriggers = false;
    }

    public void Init(Vector2 position, Vector2 dir, float speed, int damage, float lifetime, int ignoreLayer)
    {
        transform.position = position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        dmg = damage;
        life = lifetime;
        spawnTime = Time.time;
        this.ignoreLayer = ignoreLayer;

        previousPos = transform.position;
        rb.velocity = dir.normalized * speed;
    }

    private void Update()
    {
        Raycast();
        if (Time.time - spawnTime >= life) Destroy(gameObject);
    }

    // 이동 중 충돌 체크 (총알이 너무 빠르면 터널링을 하기에 Ray로 검사)
    void Raycast()
    {
        Vector2 moveDir = (Vector2)transform.position - previousPos;
        float moveDist = moveDir.magnitude;
        if (moveDist > 0.01f)
        {
            // 데미지 충돌 체크
            RaycastHit2D[] results = new RaycastHit2D[1];
            int count = Physics2D.Raycast(previousPos, moveDir, damageFilter, results, moveDist);
            if (count > 0)
            {
                var hit = results[0];
                if (hit.collider != null)
                {
                    //충돌 처리
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    target?.TakeDamage(dmg);
                    //충돌 처리
                    if (damageHitEffect != null)
                    {
                        GameObject effect = Instantiate(damageHitEffect, hit.point, Quaternion.identity);
                        effect.transform.up = hit.normal; //법선 방향으로 이펙트 방향 설정
                    }

                    Destroy(gameObject);
                    return;
                }
            }

            count = Physics2D.Raycast(previousPos, moveDir, obstacleFilter, results, moveDist);
            if (count > 0)
            {
                var hit = results[0];
                //장애물 충돌 체크
                if (hit.collider != null)
                {
                    //충돌 처리
                    if (obstacleHitEffect != null)
                    {
                        GameObject effect = Instantiate(obstacleHitEffect, hit.point, Quaternion.identity);
                        effect.transform.up = hit.normal; //법선 방향으로 이펙트 방향 설정
                    }

                    Destroy(gameObject);
                    return;
                }
            }
            
        }

        previousPos = transform.position;
    }
}
