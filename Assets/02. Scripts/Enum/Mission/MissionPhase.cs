
public enum MissionPhase
{
    Assassination, // 암살 단계(목표 제거 중)
    Escape,        // 도주 단계(목표 완료 → 탈출 지점으로)
    Completed,     // 미션 완료(탈출/클리어 확정)
    Failed         // 실패
}
