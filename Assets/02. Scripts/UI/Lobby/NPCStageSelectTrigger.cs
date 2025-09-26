using UnityEngine;

public class NPCStageSelectTrigger : MonoBehaviour, Iinteraction
{

    public void Interaction()
    {
         UIManager.Instance.OpenUI<StageSelectDialogueUI>();
    }

}
