using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private float dmg;
    private float life;
    private float spawnTime;
    private int ignoreLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // 간단하게 Trigger 충돌만
    }

    public void Init(Vector2 position, Vector2 dir, float speed, float damage, float lifetime, int ignoreLayer)
    {
        transform.position = position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        dmg = damage;
        life = lifetime;
        spawnTime = Time.time;
        this.ignoreLayer = ignoreLayer;

        rb.velocity = dir.normalized * speed;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= life) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == ignoreLayer) return;

        // 상대 오브젝트에 TakeDamage(float) 있으면 데미지 전달
        other.SendMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}
