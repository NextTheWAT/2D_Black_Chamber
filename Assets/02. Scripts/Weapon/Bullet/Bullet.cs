using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private LayerMask damageLayers = ~0; // �⺻: ���� ���
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private GameObject damageHitEffect; // �� �浹 ����Ʈ ������
    [SerializeField] private GameObject obstacleHitEffect; // ��ֹ� �浹 ����Ʈ ������

    private int dmg;
    private float life;
    private float spawnTime;
    private int ignoreLayer;
    private Vector2 previousPos;
    private ContactFilter2D damageFilter; // ������ �浹 ����
    private ContactFilter2D obstacleFilter; // ��ֹ� �浹 ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // �����ϰ� Trigger �浹��

        // Ʈ���Ŵ� �浹 ���ϵ��� ���� ����
        damageFilter = MakeFilter(damageLayers);
        obstacleFilter = MakeFilter(obstacleLayers);
    }
    private ContactFilter2D MakeFilter(LayerMask mask)
    {
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        filter.useTriggers = false;
        return filter;
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
        CheckCollision();
        if (Time.time - spawnTime >= life) Destroy(gameObject);
    }

    // �̵� �� �浹 üũ (�Ѿ��� �ʹ� ������ �ͳθ��� �ϱ⿡ Ray�� �˻�)
    private void CheckCollision()
    {
        Vector2 moveDir = (Vector2)transform.position - previousPos;
        float moveDist = moveDir.magnitude;
        previousPos = transform.position;

        if (moveDist <= 0.01f) return;

        // ������ �浹
        if (TryRaycastHit(damageFilter, moveDir, moveDist, out RaycastHit2D damageHit))
        {
            HandleDamageHit(damageHit);
            return;
        }

        // ��ֹ� �浹
        if (TryRaycastHit(obstacleFilter, moveDir, moveDist, out RaycastHit2D obstacleHit))
        {
            HandleObstacleHit(obstacleHit);
            return;
        }
    }

    private bool TryRaycastHit(ContactFilter2D filter, Vector2 dir, float dist, out RaycastHit2D hit)
    {
        RaycastHit2D[] results = new RaycastHit2D[1];
        int count = Physics2D.Raycast(previousPos, dir, filter, results, dist);
        hit = results[0];
        return count > 0 && hit.collider != null;
    }

    private void HandleDamageHit(RaycastHit2D hit)
    {
        hit.collider.GetComponent<IDamageable>()?.TakeDamage(dmg);

        if (damageHitEffect != null)
            SpawnEffect(damageHitEffect, hit);

        Destroy(gameObject);
    }

    private void HandleObstacleHit(RaycastHit2D hit)
    {
        if (obstacleHitEffect != null)
            SpawnEffect(obstacleHitEffect, hit);

        Destroy(gameObject);
    }

    private void SpawnEffect(GameObject effectPrefab, RaycastHit2D hit)
    {
        GameObject effect = Instantiate(effectPrefab, hit.point, Quaternion.identity);
        effect.transform.up = hit.normal; // ���� ���� ����
    }
}
