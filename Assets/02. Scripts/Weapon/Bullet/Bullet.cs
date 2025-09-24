using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Hit Filter")]
    [SerializeField] private LayerMask damageLayers = ~0; // �⺻: ���� ���
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private GameObject damageHitEffect; // �� �浹 ����Ʈ ������
    [SerializeField] private GameObject obstacleHitEffect; // ��ֹ� �浹 ����Ʈ ������

    private int dmg;
    private float life;
    private float spawnTime;
    private int ignoreLayer;
    private Vector2 previousPos;
    private float noiseRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // �����ϰ� Trigger �浹��
    }

    public void Init(Vector2 position, Vector2 dir, float speed, int damage, float noiseRange, float lifetime, int ignoreLayer, string bulletTag)
    {
        transform.position = position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        dmg = damage;
        life = lifetime;
        spawnTime = Time.time;
        this.ignoreLayer = ignoreLayer;
        this.noiseRange = noiseRange;
        gameObject.tag = bulletTag;

        previousPos = transform.position;
        rb.velocity = dir.normalized * speed;
    }

    private void Update()
    {
        Raycast();
        if (Time.time - spawnTime >= life) Destroy(gameObject);
    }

    // �̵� �� �浹 üũ (�Ѿ��� �ʹ� ������ �ͳθ��� �ϱ⿡ Ray�� �˻�)
    void Raycast()
    {
        Vector2 moveDir = (Vector2)transform.position - previousPos;
        float moveDist = moveDir.magnitude;
        if (moveDist > 0.01f)
        {
            // ������ �浹 üũ
            RaycastHit2D hit = Physics2D.Raycast(previousPos, moveDir, moveDist, damageLayers);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(gameObject.tag)) return; //�ڱ� �ڽŰ� ���� �±״� ����

                //�浹 ó��
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                target?.TakeDamage(dmg);
                //�浹 ó��
                if (damageHitEffect != null)
                {
                    GameObject effect = Instantiate(damageHitEffect, hit.point, Quaternion.identity);
                    effect.transform.up = hit.normal; //���� �������� ����Ʈ ���� ����
                }

                NoiseManager.Instance.EmitNoise(hit.point, noiseRange);
                Destroy(gameObject);
                return;
            }

            //��ֹ� �浹 üũ
            hit = Physics2D.Raycast(previousPos, moveDir, moveDist, obstacleLayers);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(gameObject.tag)) return; //�ڱ� �ڽŰ� ���� �±״� ����

                //�浹 ó��
                if (obstacleHitEffect != null)
                {
                    GameObject effect = Instantiate(obstacleHitEffect, hit.point, Quaternion.identity);
                    effect.transform.up = hit.normal; //���� �������� ����Ʈ ���� ����
                }

                NoiseManager.Instance.EmitNoise(hit.point, noiseRange);
                Destroy(gameObject);
                return;
            }
        }

        previousPos = transform.position;
    }
}
