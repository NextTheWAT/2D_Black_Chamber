using System.Collections.Generic;
using UnityEngine;

// Unity 에디터 메뉴에 생성 항목 추가
[CreateAssetMenu(fileName = "EnemyDialogueData", menuName = "Enemy/Dialogue Data", order = 1)]
public class EnemyDialogueData : ScriptableObject
{
    // 1. 상태별 대사 목록 정의
    [Header("Non-Combat Dialogue")]
    [Tooltip("의심 상태 (SuspectState) 대사 목록")]
    public List<string> SuspectDialogue = new List<string> { "뭐야?", "뭐지?" };

    [Tooltip("수색 상태 (InvestigateState) 대사 목록")]
    public List<string> InvestigateDialogue = new List<string> {
        "의심스럽군",
        "아무도 없어?",
        "거기 누구야?",
        "뭔가 보였는데"
    };

    [Tooltip("복귀 상태 (ReturnState) 대사 목록")]
    public List<string> ReturnDialogue = new List<string> {
        "잘 못 봤나..",
        "뭐였지.."
    };

    [Header("Combat Dialogue - ID Specific")]
    [Tooltip("ID 1002001 (돌격병) 전투 대사 목록")]
    public List<string> SpecificCombatDialogue_1002001 = new List<string> {
        "공격!",
        "죽어!",
        "돌격!",
        "덤벼!",
        "으아아!"
    };

    [Tooltip("ID 1002002 (엄폐병) 전투 대사 목록")]
    public List<string> SpecificCombatDialogue_1002002 = new List<string> {
        "침입자다!",
        "적 발견!",
        "들어와봐!",
        "지원바람!"
    };

    // 2. 외부에서 대사 리스트를 가져오는 함수
    /// 현재 Enemy의 ID와 상태에 맞는 대사 목록을 반환
    public List<string> GetDialogueList(int enemyId, EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Suspect:
                return SuspectDialogue;
            case EnemyState.Investigate:
                return InvestigateDialogue;
            case EnemyState.Return:
                return ReturnDialogue;
            case EnemyState.Combat:
                // ID에 따라 특정 전투 대사 목록 반환
                if (enemyId == 1002001)
                {
                    return SpecificCombatDialogue_1002001;
                }
                else if (enemyId == 1002002)
                {
                    return SpecificCombatDialogue_1002002;
                }

                return SpecificCombatDialogue_1002001; // 임시로 1002001 대사를 기본값으로 사용

            case EnemyState.Patrol:
            default:
                return new List<string>(); // 순찰 상태 등 대사가 없는 상태는 빈 리스트 반환
        }
    }
}

// FSM 상태 구분을 위해 Enemy.cs에 enum이 없다면 여기에 정의
public enum EnemyState
{
    None,
    Patrol,      // 순찰 (대사 없음)
    Suspect,     // 의심 (SuspectState)
    Investigate, // 수색/조사 (InvestigateState)
    Return,      // 복귀 (ReturnState)
    Combat       // 전투 (AttackState)
}
