using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    [Header("Invincibility")]
    [SerializeField] private bool useInvincible = false; // 무적 사용 여부
    [SerializeField] private float invincibleDuration = 1.0f; // 피격 후 무적 시간
    private bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    public UnityEvent<int, int> OnHealthChanged; // (현재 체력, 최대 체력)
    public UnityEvent OnDie;
    public UnityEvent OnInvincibleStart;
    public UnityEvent OnInvincibleEnd;


    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Init(int hp)
    {
        maxHealth = hp;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return; // 죽은 상태면 무시
        if (IsInvincible) return; // 무적 상태면 무시

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(damage + " 데미지 받음");

        if (currentHealth <= 0)
            Die();
        else
            StartInvincible();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDie?.Invoke();
    }

    private void StartInvincible()
    {
        if (!useInvincible) return; // 무적 사용 안 하면 무시
        if (isInvincible) return; // 이미 무적 상태면 무시
        StartCoroutine(InvincibleCoroutine());
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        OnInvincibleStart?.Invoke();

        yield return new WaitForSeconds(invincibleDuration);

        isInvincible = false;
        OnInvincibleEnd?.Invoke();
    }

}
