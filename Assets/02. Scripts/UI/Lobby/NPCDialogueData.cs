using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPCDialogueData", fileName = "NewNPCDialogue")]
public class NPCDialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueEntry
    {
        [TextArea] public List<string> lines;
    }

    public string npcName;
    public List<string> randomDialogues; // 재방문/일반 상황 랜덤 대사
    public List<DialogueEntry> firstMeetDialogues; // 첫 만남
    public List<DialogueEntry> firstFailDialogues; // 첫 임무 실패 후
    public List<DialogueEntry> failDialogues;      // 이후 임무 실패 후
    public List<DialogueEntry> availableStageDialogues;   // 플레이 가능한 스테이지 선택 시
    public List<DialogueEntry> lockedStageDialogues;      // 플레이 불가능한 스테이지 선택 시
    public List<DialogueEntry> stage1ClearDialogues;
    public List<DialogueEntry> stage2ClearDialogues;
    public List<DialogueEntry> stage3ClearDialogues;
}
