using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MoneyManager : Singleton<MoneyManager>
{
    [Header("초기 머니")]
    [SerializeField] private int startingBalance = 0;

    [SerializeField] private int balance = 0;
    public int Balance => balance;

    public UnityEvent OnMoneyChanged = new UnityEvent();

    [SerializeField] private TMP_Text balanceText;

    private void Awake()
    {
        // 첫 씬에서만 적용하도록 단순 처리 (원한다면 저장/로드로 대체)
        if (balance == 0 && startingBalance > 0)
            Set(startingBalance);

        UpdateUI();
    }
    private void OnEnable()
    {
        OnMoneyChanged.AddListener(UpdateUI);
    }
    private void OnDisable()
    {
        OnMoneyChanged.RemoveListener(UpdateUI);
    }

    public void Set(int amount)
    {
        balance = Mathf.Max(0, amount);
        OnMoneyChanged.Invoke();
    }

    public void Add(int amount)
    {
        if (amount == 0) return;
        balance = Mathf.Max(0, balance + amount);
        OnMoneyChanged.Invoke();
    }

    public bool TrySpend(int cost)
    {
        if (cost <= 0) return true;
        if (balance < cost) return false;
        balance -= cost;
        OnMoneyChanged.Invoke();
        return true;
    }

    private void UpdateUI()
    {
        if (balanceText != null)
            balanceText.text = balance.ToString();
    }
}
