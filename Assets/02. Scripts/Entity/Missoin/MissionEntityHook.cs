using UnityEngine;

[DisallowMultipleComponent]
public class MissionEntityHook : MonoBehaviour
{
    [SerializeField] private MissionEntityKind kind = MissionEntityKind.Enemy;
    private void OnEnable()
    {
        if (MissionManager.Instance == null) return;
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetActivated();
        else MissionManager.Instance.EnemyActivated();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance == null) return;
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetDeactivated();
        else MissionManager.Instance.EnemyDeactivated();
    }
}
