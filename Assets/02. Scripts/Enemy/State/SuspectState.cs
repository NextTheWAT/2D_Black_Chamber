using UnityEngine;

public class SuspectState : BaseState
{
    private readonly float suspicionBuildTime = 3f;
    private float currentDetectionRange = 0f;
    private float suspicionElapsedTime = 0f;

    public SuspectState(Enemy owner, float suspicionBuildTime) : base(owner)
    {
        this.suspicionBuildTime = suspicionBuildTime;
    }

    // 1. Ÿ���� ���� �þ߰Ÿ����� ������ ���� ����
    // 2. ���� ���� �þ߰Ÿ��� ���� �þ߰Ÿ����� õõ�� ������.
    // 3. Ÿ���� ���� �þ߰Ÿ� ���� ������ Ÿ������ ������

    public override void Enter()
    {
        suspicionElapsedTime = 0f;
        currentDetectionRange = 0f;
        owner.SetBackwardLightRadius(0f);
        ConditionalLogger.Log("Enter Suspect State");
    }

    public override void Update()
    {
        Transform target = owner.FindSuspiciousTarget();

        suspicionElapsedTime += Time.deltaTime;
        float ratio = suspicionElapsedTime / suspicionBuildTime;
        owner.SetBackwardLightRadius(ratio);

        currentDetectionRange = Mathf.Lerp(0, owner.ViewDistance, ratio);
        owner.FindTarget(owner.ViewAngle, currentDetectionRange);
        if(target)
            owner.LookAt(target.position);
    }

    public override void Exit()
    {
        owner.SetBackwardLightRadius(0f);
        ConditionalLogger.Log("Exit Suspect State");
    }

}
