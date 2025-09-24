using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    public interface IInteractable
    {
        void Interaction(GameObject player);
    }

    //public int health = 100;
    public GameObject ammoPickupPrefab;  // 인터랙션 프리팹

    //public void TakeDamage(int damage)
    //{
    //    health -= damage;
    //    if (health <= 0)
    //    {
    //        Die();
    //    }
    //}

    //void Die()
    //{
    //    // 적 위치에 탄창 프리팹 생성
    //    Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
    //    Destroy(gameObject); // 적 사라짐
    //}
}

