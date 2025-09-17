using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private int maxStamina = 100;

    public float currentStamina;

    [SerializeField] private int costPerSecond = 10;    // �Ҹ�
    [SerializeField] private int regenStamina = 5;
    [SerializeField] private float regenDelay = 1.5f;   // ȸ��������

    private bool isRunning;
    private float lastUseTime;


    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (isRunning)
        {
            Consumption();
        }
        else
        {
            Recovery();
        }
    }

    public void Running()
    {
        if (currentStamina > 0)
            isRunning = true;
        else
            isRunning = false;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    private void Consumption()
    {
        if (currentStamina > 0)
        {
            //���׹̳� �Ҹ�
            currentStamina -= costPerSecond * Time.deltaTime;

            // ���׹̳� �Ҹ� ������ �ð�
            lastUseTime = Time.time;
        }

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            isRunning = false;
        }
    }

    private void Recovery()
    {
        // ������ ����� �����̰� ������
        if (Time.time >= lastUseTime + regenDelay && currentStamina < maxStamina)
        {
            // �ð��� ���ݾ� ���׹̳� ȸ��
            currentStamina += regenStamina * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }
    }
}
