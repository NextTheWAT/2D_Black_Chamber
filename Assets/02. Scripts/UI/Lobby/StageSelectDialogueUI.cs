using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using Esper.Freeloader;

static class ProgressFlags
{
    public static bool Get(string key) => PlayerPrefs.GetInt(key, 0) == 1;
    public static void Set(string key, bool v)
    {
        PlayerPrefs.SetInt(key, v ? 1 : 0);
        PlayerPrefs.Save();
    }

    public const string FirstMeetDone = "NPC_FirstMeetDone";      // 최초 대화 1회 처리용
    public static string StageCleared(int n) => $"Stage{n}_Cleared"; // 예) Stage1_Cleared
    public static string ClearDialogueDone(int n) => $"Stage{n}_ClearDialogueDone";
}

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
    [SerializeField] private Button stage4Button;
    [SerializeField] private Button stage5Button;
    [SerializeField] private Button exitButton;

    [Header("Scene Names")]
    [SerializeField] private string stage1SceneName = "ProtoTypeScene";
    [SerializeField] private string stage2SceneName = "ProtoTypeScene";
    [SerializeField] private string stage3SceneName = "ProtoTypeScene";
    [SerializeField] private string stage4SceneName = "ProtoTypeScene";
    [SerializeField] private string stage5SceneName = "ProtoTypeScene";

    [Header("Typing Settings")]
    [SerializeField] private float typingDelay = 0.05f;

    [Header("TV Effect Objects")]
    [SerializeField] private GameObject tvOnEffect1;
    [SerializeField] private GameObject tvOnEffect2;
    [SerializeField] private GameObject tvOnEffect3;
    [SerializeField] private GameObject tvOnEffect4;
    [SerializeField] private GameObject tvOnEffect5;
    [SerializeField] private GameObject tvOnEffect6;
    [SerializeField] private float effectDelay = 0.2f;

    private List<string> currentLines;
    private int currentIndex = -1;
    private bool inSequence = false;
    private Coroutine typingCoroutine;

    private bool _isEffectPlaying = false;
    private bool _hasOpenExecuted = false;

    private DialogueBlockerController blockerController;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (_hasOpenExecuted) return;
        _hasOpenExecuted = true;

        // 1. 블로커 컴포넌트 찾기
        blockerController = GetComponentInChildren<DialogueBlockerController>(true);

        if (blockerController == null)
        {
            Debug.LogError("StageSelectDialogueUI: DialogueBlockerController를 자식에서 찾을 수 없어 블로커 작동 불가.", this);
        }
        else
        {
            blockerController.SetBlockerActive(false);
        }

        // UI가 열리면, 실제 UI 내용을 보여주기 전에 TV 켜짐 효과 시퀀스 시작
        StartCoroutine(PlayTVOnEffectSequence());
    }

    private IEnumerator PlayTVOnEffectSequence()
    {
        _isEffectPlaying = true;

        if (tvOnEffect1) tvOnEffect1.SetActive(false);
        if (tvOnEffect2) tvOnEffect2.SetActive(false);
        if (tvOnEffect3) tvOnEffect3.SetActive(false);
        if (tvOnEffect4) tvOnEffect4.SetActive(false);
        if (tvOnEffect5) tvOnEffect5.SetActive(false);
        if (tvOnEffect6) tvOnEffect6.SetActive(false);

        if (tvOnEffect1)
        {
            tvOnEffect1.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect1.SetActive(false);
        }
        if (tvOnEffect2)
        {
            tvOnEffect2.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect2.SetActive(false);
        }
        if (tvOnEffect3)
        {
            tvOnEffect3.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect3.SetActive(false);
        }
        if (tvOnEffect4)
        {
            tvOnEffect4.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect4.SetActive(false);
        }
        if (tvOnEffect5)
        {
            tvOnEffect5.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect5.SetActive(false);
        }
        if (tvOnEffect6)
        {
            tvOnEffect6.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay * 1.5f);
            tvOnEffect6.SetActive(false);
        }

        _isEffectPlaying = false;

        ExecuteOnOpenLogic();
    }

    private void ExecuteOnOpenLogic()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;

        if (!Initialized)
        {
            if (stage1Button)
            {
                stage1Button.onClick.RemoveAllListeners();
                stage1Button.onClick.AddListener(() => TryStartStage(1, stage1SceneName));
            }
            if (stage2Button)
            {
                stage2Button.onClick.RemoveAllListeners();
                stage2Button.onClick.AddListener(() => TryStartStage(2, stage2SceneName));
            }
            if (stage3Button)
            {
                stage3Button.onClick.RemoveAllListeners();
                stage3Button.onClick.AddListener(() => TryStartStage(3, stage3SceneName));
            }
            if (stage4Button)
            {
                stage4Button.onClick.RemoveAllListeners();
                stage4Button.onClick.AddListener(() => TryStartStage(4, stage4SceneName));
            }
            if (stage5Button)
            {
                stage5Button.onClick.RemoveAllListeners();
                stage5Button.onClick.AddListener(() => TryStartStage(5, stage5SceneName));
            }
            if (exitButton)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(RequestClose);
            }
            Initialized = true;
        }

        if (dialogueData != null && nameText)
            nameText.text = dialogueData.npcName;

        ApplyButtonStatesWithColor();

        if (TryPlayPendingClearDialogue())
            return;

        if (nameText) nameText.raycastTarget = false;
        if (lineText) lineText.raycastTarget = false;

        // First Meet는 '최초 1회만'
        bool firstMeetDone = ProgressFlags.Get(ProgressFlags.FirstMeetDone);

        if (!firstMeetDone &&
            dialogueData != null &&
            dialogueData.firstMeetDialogues != null &&
            dialogueData.firstMeetDialogues.Count > 0)
        {
            // firstMeetDialogues 대사에는 true를 넘겨 블로커 활성화
            StartSequence(dialogueData.firstMeetDialogues[0].lines, true);
            ProgressFlags.Set(ProgressFlags.FirstMeetDone, true);
        }
        else
        {
            ShowRandomOneLiner();
        }
    }

    private void Update()
    {
        if (_isEffectPlaying || !inSequence) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (lineText && currentLines != null && currentIndex < currentLines.Count)
                {
                    lineText.text = currentLines[currentIndex];
                }
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartSequence(List<string> lines, bool shouldBlockUI = false)
    {
        if (lines == null || lines.Count == 0)
        {
            ShowRandomOneLiner();
            return;
        }

        currentLines = lines;
        currentIndex = -1;
        inSequence = true;

        // 블록킹이 필요한 경우 블로커 활성화
        if (shouldBlockUI && blockerController != null)
        {
            blockerController.SetBlockerActive(true);
        }

        NextLine();
    }

    private void NextLine()
    {
        currentIndex++;

        if (currentLines == null || currentIndex >= currentLines.Count)
        {
            EndSequence();
            return;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (lineText) lineText.text = "";

        typingCoroutine = StartCoroutine(TypeSentence(currentLines[currentIndex]));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            lineText.text += letter;
            yield return new WaitForSecondsRealtime(typingDelay);
        }

        typingCoroutine = null;
    }


    private void EndSequence()
    {
        inSequence = false;
        currentLines = null;
        currentIndex = -1;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 대화가 끝나면 블로커 비활성화 (대화 종류 상관없이)
        if (blockerController != null)
        {
            blockerController.SetBlockerActive(false);
        }

        ShowRandomOneLiner();
    }

    private void ShowRandomOneLiner()
    {
        if (dialogueData != null &&
            dialogueData.randomDialogues != null &&
            dialogueData.randomDialogues.Count > 0)
        {
            if (lineText)
                lineText.text = dialogueData.randomDialogues[Random.Range(0, dialogueData.randomDialogues.Count)];
        }
        else
        {
            if (lineText)
                lineText.text = "";
        }
    }
    public void RequestClose()
    {
        OnClose();
        UIManager.Instance.CloseUI<StageSelectDialogueUI>();
    }

    protected override void OnClose()
    {
        Time.timeScale = 1f;
        _hasOpenExecuted = false;
    }

    private void TryStartStage(int stageNumber, string sceneName)
    {
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);

        if (!IsUnlocked(stageNumber))
        {
            // 잠겨있으면 Locked 시퀀스만 출력, 씬 이동 없음 (블록킹 X)
            if (dialogueData != null &&
        dialogueData.lockedStageDialogues != null &&
        dialogueData.lockedStageDialogues.Count > 0 &&
        dialogueData.lockedStageDialogues[0].lines != null &&
        dialogueData.lockedStageDialogues[0].lines.Count > 0)
            {
                // lockedStageDialogues는 블록킹 하지 않음 (false)
                StartSequence(dialogueData.lockedStageDialogues[0].lines, false);
            }
            else
            {
                StartSequence(new List<string> { "먼저 해야 할 일이 있지 않아?" }, false);
            }
            return;
        }

        // 열려 있으면 정상 로드
        LoadStage(sceneName);
    }

    private bool IsUnlocked(int stage)
    {
        if (stage <= 1) return true;
        return ProgressFlags.Get(ProgressFlags.StageCleared(stage - 1));
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

        LoadingScreen.Instance.Load(sceneName);
    }

    // 스테이지 클리어 직후 ‘1회성 대사’를 재생. 재생하면 true.
    private bool TryPlayPendingClearDialogue()
    {
        // 1스테이지 클리어 예약
        if (PlayerPrefs.GetInt("Stage1_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage1_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (!ProgressFlags.Get(ProgressFlags.ClearDialogueDone(1)))
            {
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(1), true);

                if (dialogueData?.stage1ClearDialogues != null &&
                    dialogueData.stage1ClearDialogues.Count > 0 &&
                    dialogueData.stage1ClearDialogues[0].lines?.Count > 0)
                {
                    // stage1ClearDialogues 대사에는 true를 넘겨 블로커 활성화
                    StartSequence(dialogueData.stage1ClearDialogues[0].lines, true);
                    return true;
                }
            }
        }
        // 2스테이지 클리어 예약 (나머지 스테이지도 동일하게 true로 수정)
        if (PlayerPrefs.GetInt("Stage2_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage2_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (!ProgressFlags.Get(ProgressFlags.ClearDialogueDone(2)))
            {
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(2), true);

                if (dialogueData?.stage2ClearDialogues != null &&
                    dialogueData.stage2ClearDialogues.Count > 0 &&
                    dialogueData.stage2ClearDialogues[0].lines?.Count > 0)
                {
                    StartSequence(dialogueData.stage2ClearDialogues[0].lines, true);
                    return true;
                }
            }
        }

        if (PlayerPrefs.GetInt("Stage3_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage3_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (!ProgressFlags.Get(ProgressFlags.ClearDialogueDone(3)))
            {
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(3), true);

                if (dialogueData?.stage3ClearDialogues != null &&
                    dialogueData.stage3ClearDialogues.Count > 0 &&
                    dialogueData.stage3ClearDialogues[0].lines?.Count > 0)
                {
                    StartSequence(dialogueData.stage3ClearDialogues[0].lines, true);
                    return true;
                }
            }
        }
        if (PlayerPrefs.GetInt("Stage4_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage4_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (!ProgressFlags.Get(ProgressFlags.ClearDialogueDone(4)))
            {
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(4), true);

                if (dialogueData?.stage4ClearDialogues != null &&
                    dialogueData.stage4ClearDialogues.Count > 0 &&
                    dialogueData.stage4ClearDialogues[0].lines?.Count > 0)
                {
                    StartSequence(dialogueData.stage4ClearDialogues[0].lines, true);
                    return true;
                }
            }
        }
        if (PlayerPrefs.GetInt("Stage5_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage5_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (!ProgressFlags.Get(ProgressFlags.ClearDialogueDone(5)))
            {
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(5), true);

                if (dialogueData?.stage5ClearDialogues != null &&
                    dialogueData.stage5ClearDialogues.Count > 0 &&
                    dialogueData.stage5ClearDialogues[0].lines?.Count > 0)
                {
                    StartSequence(dialogueData.stage5ClearDialogues[0].lines, true);
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateButton(Button button, bool unlocked)
    {
        if (button == null) return;
        button.interactable = true;
        var txt = button.GetComponentInChildren<TMP_Text>();
        if (txt) txt.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        if (!unlocked)
        {
            button.transition = Selectable.Transition.None;
        }
        else
        {
            button.transition = Selectable.Transition.ColorTint;
        }
    }

    private void ApplyButtonStatesWithColor()
    {
        UpdateButton(stage1Button, IsUnlocked(1));
        UpdateButton(stage2Button, IsUnlocked(2));
        UpdateButton(stage3Button, IsUnlocked(3));
        UpdateButton(stage4Button, IsUnlocked(4));
        UpdateButton(stage5Button, IsUnlocked(5));
    }
}