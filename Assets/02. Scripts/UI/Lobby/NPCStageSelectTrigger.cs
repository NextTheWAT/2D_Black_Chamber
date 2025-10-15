using UnityEngine;

public class NPCStageSelectTrigger : MonoBehaviour, Iinteraction
{

    public void Interaction(Transform interactor)
    {
         UIManager.Instance.OpenUI<StageSelectDialogueUI>();
    }

}
