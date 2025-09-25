using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    private int money;

    [Header("Mission Phase")]
    public int nomalState = 100;    // �⺻ 100% �ݾ�
    public int combatState = 50;    // ���� ���� = 50% �ݾ�

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
        // �� ���
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
