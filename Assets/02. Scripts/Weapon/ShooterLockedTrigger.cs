using UnityEngine;

public class ShooterLockedTrigger : MonoBehaviour
{
    public bool shooterLocked = false; // 발사 금지


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WeaponManager.Instance.SetShooterLocked(shooterLocked);
        }
    }
}
