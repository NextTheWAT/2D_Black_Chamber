using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [Header("Common")]
    public int id;
    public string enemyName;
    public int hp;
    public float speed;
    public float viewDistance;
    public float viewAngle;
    public int equipWepaon;

    [Header("Patorl")]
    public float patrolPauseTime; // 순찰 중 멈추는 시간
    public float patrolFixedAngle; // 고정 순찰 각도

    [Header("Suspect")]
    public float suspectBuildTime; // 의심 상태에서 타겟을 획득하는 데 걸리는 시간

    [Header("Investigate")]
    public float investigateStartDelay; // 조사 시작 전 대기 시간
    public float investigateDuration; // 조사 상태 지속 시간
    public float investigatePauseDuration; // 조사 중 멈추는 시간
    public float investigateRange; // 조사 중 무작위로 이동하는 범위

    [Header("Cover")]
    public float coverOffset; // 엄폐 지점 오프셋

    [Header("Attack")]
    public float rangedAttackRange; // 공격 범위
    public float desiredAttackDistance; // 적과의 원하는 공격 거리
    public int meleeAttackDamage; // 근접 공격 데미지
    public float meleeAttackRange; // 근접 공격 범위

    [Header("Retreat")]
    public float retreatHealthRatio; // 후퇴하는 체력 비율 ex) .2f = 20%
    public float retreatDistance; // 후퇴 거리
    public float returnTime; // 후퇴 후 복귀 시간

    [Header("Flee")]
    public float fleeDistance; // 도주 거리
    public float fleeDuration; // 도주 지속 시간

    [Header("Death")]
    public float deathSignalRadius; // 죽음 신호 반경
}
