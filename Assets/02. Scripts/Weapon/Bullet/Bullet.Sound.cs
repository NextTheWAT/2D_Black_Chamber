using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public partial class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask wallSoundLayer;
    [SerializeField] private LayerMask doorSoundLayer;
    [SerializeField] private LayerMask steelDoorSoundLayer;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == wallSoundLayer) {
            StructSoundManager.Instance.PlayWallAttackSound(transform.position);
        }
        else if (other.gameObject.layer == doorSoundLayer) 
        {
            StructSoundManager.Instance.PlayDoorAttackSound(transform.position);
        }
        else if (other.gameObject.layer == steelDoorSoundLayer) 
        {
            StructSoundManager.Instance.PlaySteelDoorAttackSound(transform.position);
        }
    }
}
