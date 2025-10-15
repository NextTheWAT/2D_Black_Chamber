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

    // 1. 타겟이 최종 시야거리내에 있으면 상태 시작
    // 2. 나의 현재 시야거리가 최종 시야거리까지 천천히 증가함.
    // 3. 타겟이 현재 시야거리 내에 있으면 타겟으로 지정됨

    public override void Enter()
    {
        owner.Agent.isStopped = true;
        suspicionElapsedTime = 0f;
        currentDetectionRange = 0f;
        owner.SetBackwardLightRadius(0f);
        ConditionalLogger.Log("Enter Suspect State");
    }

    public override void Update()
    {
        if (owner.HasTargetInFOV)
            owner.LookPoint = owner.TargetInFOV.position;

        suspicionElapsedTime += Time.deltaTime;
        float ratio = suspicionElapsedTime / suspicionBuildTime;
        owner.SetBackwardLightRadius(ratio);

        currentDetectionRange = Mathf.Lerp(0, owner.ViewDistance, ratio);
        owner.FindTarget(owner.ViewAngle, currentDetectionRange);

        if (owner.IsHit)
            owner.Target = GameManager.Instance.Player;

        if (owner.HasTarget)
        {
            ConditionalLogger.Log("SuspectState: Target Acquired");
            GameManager.Instance.StartCombatAfterDelay(owner);
        }
    }

    public override void Exit()
    {
        owner.Agent.isStopped = false;
        owner.SetBackwardLightRadius(0f);
        ConditionalLogger.Log("Exit Suspect State");
    }

}
