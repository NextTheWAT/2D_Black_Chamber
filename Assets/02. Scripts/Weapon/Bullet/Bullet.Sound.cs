using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D))]
public partial class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask wallSoundLayer;
    [SerializeField] private LayerMask doorSoundLayer;
    [SerializeField] private LayerMask steelDoorSoundLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        int mask = 1 << other.gameObject.layer;

        if ((wallSoundLayer.value & mask) != 0)
        {
            StructSoundManager.Instance.PlayWallAttackSound(transform.position);
            Debug.Log("Wall Hit Sound Played");
        }
        else if ((doorSoundLayer.value & mask) != 0)
        {
            StructSoundManager.Instance.PlayDoorAttackSound(transform.position);
        }
        else if ((steelDoorSoundLayer.value & mask) != 0)
        {
            StructSoundManager.Instance.PlaySteelDoorAttackSound(transform.position);
        }
    }
}
