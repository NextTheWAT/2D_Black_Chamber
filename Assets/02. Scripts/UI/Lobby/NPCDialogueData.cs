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
    public List<string> randomDialogues; // ��湮/�Ϲ� ��Ȳ ���� ���
    public List<DialogueEntry> firstMeetDialogues; // ù ����
    public List<DialogueEntry> firstFailDialogues; // ù �ӹ� ���� ��
    public List<DialogueEntry> failDialogues;      // ���� �ӹ� ���� ��
    public List<DialogueEntry> availableStageDialogues;   // �÷��� ������ �������� ���� ��
    public List<DialogueEntry> lockedStageDialogues;      // �÷��� �Ұ����� �������� ���� ��
    public List<DialogueEntry> stage1ClearDialogues;
    public List<DialogueEntry> stage2ClearDialogues;
    public List<DialogueEntry> stage3ClearDialogues;
}
