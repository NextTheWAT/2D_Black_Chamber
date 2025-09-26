using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class StageSelectDialogueUI : UIBase
{
    [Header("Dialogue Data")]
    [SerializeField] private NPCDialogueData dialogueData;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lineText;

    [Header("Buttons")]
    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;
    [SerializeField] private Button exitButton;

    [Header("Scene Names")]
    [SerializeField] private string stage1SceneName = "ProtoTypeScene";
    [SerializeField] private string stage2SceneName = "ProtoTypeScene";
    [SerializeField] private string stage3SceneName = "ProtoTypeScene";

    protected override void OnOpen()
    {
        Time.timeScale = 0f; //일시정지

        if (!Initialized)
        {
            if (stage1Button)
            {
                stage1Button.onClick.RemoveAllListeners();
                stage1Button.onClick.AddListener(() => LoadStage(stage1SceneName));
            }
            if (stage2Button)
            {
                stage2Button.onClick.RemoveAllListeners();
                stage2Button.onClick.AddListener(() => LoadStage(stage2SceneName));
            }
            if (stage3Button)
            {
                stage3Button.onClick.RemoveAllListeners();
                stage3Button.onClick.AddListener(() => LoadStage(stage3SceneName));
            }
            if (exitButton)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(RequestClose);
            }

            Initialized = true;

            if (dialogueData != null)
            {
                if (nameText) nameText.text = dialogueData.npcName;

                var pool = dialogueData.randomDialogues;
                if (pool != null && pool.Count > 0)
                    if (lineText) lineText.text = pool[Random.Range(0, pool.Count)];
                    else
                    if (lineText) lineText.text = "";
            }
        }
    }

    public void RequestClose()
    {
        UIManager.Instance.CloseUI<StageSelectDialogueUI>();
    }

    protected override void OnClose()
    {
        Time.timeScale = 1f;
    }

    private void LoadStage(string sceneName)
    {
        Time.timeScale = 1f;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[StageSelectDialogueUI] 씬 '{sceneName}' 를 찾을 수 없음");
            return;
        }

        PlayerPrefs.SetString("LastStage", sceneName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }
}
