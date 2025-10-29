using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public partial class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall")) StructSoundManager.Instance.PlayWallAttackSound(transform.position);
        else if(other.CompareTag("Door")) StructSoundManager.Instance.PlayDoorAttackSound(transform.position);
        else if(other.CompareTag("SteelDoor")) StructSoundManager.Instance.PlaySteelDoorAttackSound(transform.position);
    }
}
