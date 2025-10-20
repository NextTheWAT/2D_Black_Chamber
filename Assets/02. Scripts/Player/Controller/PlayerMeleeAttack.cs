using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    public Vector2 hitBoxSize = Vector2.one;
    public float hitBoxDistance = .2f;
    public LayerMask enemyLayer;
    public int damageAmount = 100;

    public void MeleeAttack()
    {
        Vector2 boxOrigin = (Vector2)transform.position + (Vector2)(transform.up * hitBoxDistance);
        float angle = Vector2.SignedAngle(Vector2.right, transform.up);
        Collider2D hit = Physics2D.OverlapBox(boxOrigin, hitBoxSize, angle, enemyLayer);
        if (hit != null)
        {
            Debug.Log("Hit " + hit.name);
            hit.GetComponent<IDamageable>().TakeDamage(damageAmount);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 boxOrigin = (Vector2)transform.position + (Vector2)(transform.up * hitBoxDistance);
        Gizmos.color = Color.red;

        // 히트 박스 transform.up 기준으로 그리기
        float angle = Vector2.SignedAngle(Vector2.right, transform.up);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxOrigin, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector2.zero, hitBoxSize);
        Gizmos.matrix = Matrix4x4.identity;

    }
}
