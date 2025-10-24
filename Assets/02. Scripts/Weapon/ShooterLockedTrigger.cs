using UnityEngine;

public class ShooterLockedTrigger : MonoBehaviour
{
    public bool shooterLocked = false; // 발사 금지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandleSetShooterLocked(shooterLocked);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandleSetShooterLocked(shooterLocked);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleSetShooterLocked(false);
        }
    }

    // 안전 호출: WeaponManager.Instance가 null이면 작업을 건너뜁니다.
    private void HandleSetShooterLocked(bool locked)
    {
        var wm = WeaponManager.Instance;
        if (wm == null)
        {
            // 개발 중 원인 추적을 위해 경고 로그만 남기고 무시
            Debug.LogWarning("[ShooterLockedTrigger] WeaponManager.Instance == null, SetShooterLocked skipped.", this);
            return;
        }
        wm.SetShooterLocked(locked);        
    }
}
