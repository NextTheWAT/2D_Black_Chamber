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
    public GameObject ammoPickupPrefab;  // ���ͷ��� ������

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
    //    // �� ��ġ�� źâ ������ ����
    //    Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
    //    Destroy(gameObject); // �� �����
    //}
}

