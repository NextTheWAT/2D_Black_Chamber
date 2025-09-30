using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;

    private int currentHealth;
    public int CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;

    public UnityEvent<int, int> OnHealthChanged; // (현재 체력, 최대 체력)
    public UnityEvent OnDie;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(damage + " 데미지 받음");

        if (currentHealth <= 0)
        {
            Die();
        }
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
        GameStats.Instance.AddKill();
    }
}
