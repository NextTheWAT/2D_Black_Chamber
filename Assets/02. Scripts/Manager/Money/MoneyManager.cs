using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    private int money;

    [Header("Mission Phase")]
    public int nomalState = 100;    // 기본 100% 금액
    public int combatState = 50;    // 전투 상태 = 50% 금액

    private bool combatTrigger = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SpendMoney(int amount)
    {
        // 돈 사용
        if (money >= amount)
        {
            money -= amount;
            return;
        }
        else
            return;
    }

    public int GetMoney()
    {
        return money;
    }
}
