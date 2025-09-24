using UnityEngine;

[DisallowMultipleComponent]
public class MissionEntityHook : MonoBehaviour
{
    [SerializeField] private MissionEntityKind kind = MissionEntityKind.Enemy;
    private bool deactivated; // 중복 감소 방지

    private void OnEnable()
    {
        if (MissionManager.Instance == null) return;
        deactivated = false; // 풀링 재활성 대비
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetActivated();
        else MissionManager.Instance.EnemyActivated();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance == null || deactivated) return;
        DeactivateOnce();
    }

    // 죽었지만 GameObject는 남겨둘 때 직접 호출
    public void NotifyLogicalDeath()
    {
        if (MissionManager.Instance == null || deactivated) return;
        DeactivateOnce();
    }

    private void DeactivateOnce()
    {
        deactivated = true;
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetDeactivated();
        else MissionManager.Instance.EnemyDeactivated();
    }
}
