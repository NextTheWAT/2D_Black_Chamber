using UnityEngine;

[DisallowMultipleComponent]
public class MissionEntityHook : MonoBehaviour
{
    [SerializeField] private MissionEntityKind kind = MissionEntityKind.Enemy;
    private bool deactivated; // �ߺ� ���� ����

    public MissionEntityKind Kind { get { return kind; } } //�ϻ� ���/�� ����
    public bool IsDeactivated { get { return deactivated; } } //�̹� ���� �ƴ���

    private void OnEnable()
    {
        if (MissionManager.Instance == null) return;
        deactivated = false; // Ǯ�� ��Ȱ�� ���
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetActivated();
        else MissionManager.Instance.EnemyActivated();
    }

    private void OnDisable()
    {
        if (MissionManager.Instance == null || deactivated) return;
        DeactivateOnce();
    }

    // �׾����� GameObject�� ���ܵ� �� ���� ȣ��
    public void NotifyLogicalDeath()
    {
        if (MissionManager.Instance == null || deactivated) return;
        DeactivateOnce();
        GameStats.Instance.AddKill();
    }

    private void DeactivateOnce()
    {
        deactivated = true;
        if (kind == MissionEntityKind.AssassinationTarget) MissionManager.Instance.TargetDeactivated();
        else MissionManager.Instance.EnemyDeactivated();
    }
}
