using UnityEngine;

public class NPCStageSelectTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private bool _playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) _playerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) _playerInRange = false;
    }

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(interactKey))
        {
            UIManager.Instance.OpenUI<StageSelectDialogueUI>();
        }
    }
}
