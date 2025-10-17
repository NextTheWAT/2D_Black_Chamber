using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBag : MonoBehaviour, Iinteraction
{
    private GameObject player;    // 플레이어 Transform
    private Health health;

    public int healAmount = 30; // 회복량


    void Start()
    {
        // Inspector에 player를 안 넣으면 씬에서 자동으로 찾기
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
