using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBag : MonoBehaviour, Iinteraction
{
    private GameObject player;    // �÷��̾� Transform
    private Health health;

    public int healAmount = 30; // ȸ����


    void Start()
    {
        // Inspector�� player�� �� ������ ������ �ڵ����� ã��
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            health = player.GetComponent<Health>();
        }
    }

    public void Interaction(Transform interactor)
    {
        PickUp();
    }

    public void PickUp()
    {
        StructSoundManager.Instance.PlayPickUpSound(transform.position);

        health.Heal(healAmount);

        Destroy(gameObject);
    }


}
