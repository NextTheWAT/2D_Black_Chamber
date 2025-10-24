using UnityEngine;
using UnityEngine.Events;

public class MoneyManager : Singleton<MoneyManager>
{
    [Header("초기 머니")]
    [SerializeField] private int startingBalance = 0;

    [SerializeField] private int balance = 0;
    public int Balance => balance;

    public UnityEvent OnChanged = new UnityEvent();

    private void Awake()
    {
        // 첫 씬에서만 적용하도록 단순 처리 (원한다면 저장/로드로 대체)
        if (balance == 0 && startingBalance > 0)
            Set(startingBalance);
    }

    public void Set(int amount)
    {
        balance = Mathf.Max(0, amount);
        OnChanged.Invoke();
    }

    public void Add(int amount)
    {
        if (amount == 0) return;
        balance = Mathf.Max(0, balance + amount);
        OnChanged.Invoke();
    }

    public bool TrySpend(int cost)
    {
        if (cost <= 0) return true;
        if (balance < cost) return false;
        balance -= cost;
        OnChanged.Invoke();
        return true;
    }
}
