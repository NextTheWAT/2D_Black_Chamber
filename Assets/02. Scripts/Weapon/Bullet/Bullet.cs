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
    //[SerializeField] private bool useTagFilter = false;
    //[SerializeField] private string[] damageTags;         // 예: "Enemy", "Player"

    private int dmg;
    private float life;
    private float spawnTime;
    private int ignoreLayer;
    private Vector2 previousPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // 간단하게 Trigger 충돌만
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
            RaycastHit2D hit = Physics2D.Raycast(previousPos, moveDir, moveDist, damageLayers);
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

                NoiseManager.Instance.EmitNoise(hit.point, 1.0f);
                Destroy(gameObject);
                return;
            }

            //장애물 충돌 체크
            hit = Physics2D.Raycast(previousPos, moveDir, moveDist, obstacleLayers);
            if (hit.collider != null)
            {
                //충돌 처리
                if (obstacleHitEffect != null)
                {
                    GameObject effect = Instantiate(obstacleHitEffect, hit.point, Quaternion.identity);
                    effect.transform.up = hit.normal; //법선 방향으로 이펙트 방향 설정
                }

                NoiseManager.Instance.EmitNoise(hit.point, 1.0f);
                Destroy(gameObject);
                return;
            }
        }

        previousPos = transform.position;
    }
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        //1. 발사자 레이어는 무시
        if (other.gameObject.layer == ignoreLayer) return;
        ConditionalLogger.Log("Bullet hit: " + other.name);

        //2. 레이어 필터링
        int otherLayerBit = 1 << other.gameObject.layer;
        if((damageLayers.value & otherLayerBit) == 0) return; // 데미지 레이어 필터링

        // 3) 태그 필터
        //if (useTagFilter && (damageTags == null || damageTags.Length == 0))
        //    return;
        //if (useTagFilter)
        //{
        //    bool tagOK = false;
        //    for (int i = 0; i < damageTags.Length; i++)
        //    {
        //        if (other.CompareTag(damageTags[i])) { tagOK = true; break; }
        //    }
        //    if (!tagOK) return;
        //}

        //인터페이스로 데미지 전달
        IDamageable target = other.GetComponent<IDamageable>();
        target?.TakeDamage(dmg);

        Destroy(gameObject);
    }
    */
}
