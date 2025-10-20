using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // F키 입력 읽기용

// =============================
// ▶ 진행 플래그 유틸 (PlayerPrefs)
//  - First Meet 1회 처리
//  - 스테이지 잠금 해제 판단(다음 단계에서 실제 세팅 붙일 예정)
// =============================
static class ProgressFlags
{
    public static bool Get(string key) => PlayerPrefs.GetInt(key, 0) == 1;
    public static void Set(string key, bool v)
    {
        PlayerPrefs.SetInt(key, v ? 1 : 0);
        PlayerPrefs.Save();
    }

    public const string FirstMeetDone = "NPC_FirstMeetDone";       // 최초 대화 1회 처리용
    public static string StageCleared(int n) => $"Stage{n}_Cleared"; // 예) Stage1_Cleared
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
    [SerializeField] private Button exitButton;

    [Header("Scene Names")]
    [SerializeField] private string stage1SceneName = "ProtoTypeScene";
    [SerializeField] private string stage2SceneName = "ProtoTypeScene";
    [SerializeField] private string stage3SceneName = "ProtoTypeScene";

    // =========================
    // 대사 진행 상태 변수
    // =========================
    private List<string> currentLines; // 현재 진행 중 "한 세트"의 줄들
    private int currentIndex = -1;     // 다음에 보여줄 줄 인덱스
    private bool inSequence = false;   // 대사 진행 중 여부

    private void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        Time.timeScale = 0f; // 일시정지
        Cursor.visible = true;

        if (!Initialized)
        {
            // 씬 바로 로드(X) → TryStartStage로 잠금 검사 후 로드(O)
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

            if (exitButton)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(RequestClose);
            }

            Initialized = true;
        }

        // NPC 이름 표시
        if (dialogueData != null && nameText)
            nameText.text = dialogueData.npcName;

        ApplyButtonStatesWithColor();

        if (TryPlayPendingClearDialogue())
            return;

        if (nameText) nameText.raycastTarget = false;
        if (lineText) lineText.raycastTarget = false;

        // =========================
        // ▶ 핵심: First Meet는 '최초 1회만'
        //   - 아직 안 봤으면 First Meet 시퀀스를 한 줄씩 보여주고 바로 완료 처리
        //   - 이미 봤으면 랜덤 멘트 1줄만 표시
        // =========================
        bool firstMeetDone = ProgressFlags.Get(ProgressFlags.FirstMeetDone);

        if (!firstMeetDone &&
            dialogueData != null &&
            dialogueData.firstMeetDialogues != null &&
            dialogueData.firstMeetDialogues.Count > 0)
        {
            StartSequence(dialogueData.firstMeetDialogues[0].lines);
            ProgressFlags.Set(ProgressFlags.FirstMeetDone, true); // 다음부턴 나오지 않게
        }
        else
        {
            ShowRandomOneLiner();
        }
    }

    private void Update()
    {
        // F키로 다음 줄
        if (!inSequence) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            NextLine();
        }
    }

    // =========================
    // 대사 진행 메서드
    // =========================
    private void StartSequence(List<string> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            ShowRandomOneLiner();
            return;
        }

        currentLines = lines;
        currentIndex = -1;
        inSequence = true;

        NextLine(); // 첫 줄 즉시 출력
    }

    private void NextLine()
    {
        currentIndex++;

        if (currentLines == null || currentIndex >= currentLines.Count)
        {
            EndSequence();
            return;
        }

        if (lineText)
            lineText.text = currentLines[currentIndex];
    }

    private void EndSequence()
    {
        inSequence = false;
        currentLines = null;
        currentIndex = -1;

        // 세트가 끝나면 기본 랜덤 멘트 1줄로 복귀
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
        UIManager.Instance.CloseUI<StageSelectDialogueUI>();
    }

    protected override void OnClose()
    {
        Time.timeScale = 1f;
    }

    // =========================
    // 스테이지 시작 시도 (잠금 검사 → 대사 or 로드)
    // =========================
    private void TryStartStage(int stageNumber, string sceneName)
    {
        if (!IsUnlocked(stageNumber))
        {
            // 잠겨있으면 Locked 시퀀스만 출력, 씬 이동 없음
            if (dialogueData != null &&
                dialogueData.lockedStageDialogues != null &&
                dialogueData.lockedStageDialogues.Count > 0 &&
                dialogueData.lockedStageDialogues[0].lines != null &&
                dialogueData.lockedStageDialogues[0].lines.Count > 0)
            {
                StartSequence(dialogueData.lockedStageDialogues[0].lines);
            }
            else
            {
                StartSequence(new List<string> { "먼저 해야 할 일이 있지 않아?" });
            }
            return;
        }

        // 열려 있으면 정상 로드
        LoadStage(sceneName);
    }

    // 1은 항상 열림, 2는 1 클리어 시, 3은 2 클리어 시 열림
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

        LoadingCanvas.LoadScene(sceneName);
    }

    /// <summary>
    /// 스테이지 클리어 직후 ‘1회성 대사’를 재생. 재생하면 true.
    /// </summary>
    private bool TryPlayPendingClearDialogue()
    {
        // 1스테이지 클리어 예약
        if (PlayerPrefs.GetInt("Stage1_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage1_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (dialogueData?.stage1ClearDialogues != null &&
                dialogueData.stage1ClearDialogues.Count > 0 &&
                dialogueData.stage1ClearDialogues[0].lines?.Count > 0)
            {
                StartSequence(dialogueData.stage1ClearDialogues[0].lines);
                return true;
            }
        }
        // 2스테이지 클리어 예약
        if (PlayerPrefs.GetInt("Stage2_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage2_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (dialogueData?.stage2ClearDialogues != null &&
                dialogueData.stage2ClearDialogues.Count > 0 &&
                dialogueData.stage2ClearDialogues[0].lines?.Count > 0)
            {
                StartSequence(dialogueData.stage2ClearDialogues[0].lines);
                return true;
            }
        }

        // 3스테이지 클리어 예약
        if (PlayerPrefs.GetInt("Stage3_ClearDialoguePending", 0) == 1)
        {
            PlayerPrefs.SetInt("Stage3_ClearDialoguePending", 0);
            PlayerPrefs.Save();

            if (dialogueData?.stage3ClearDialogues != null &&
                dialogueData.stage3ClearDialogues.Count > 0 &&
                dialogueData.stage3ClearDialogues[0].lines?.Count > 0)
            {
                StartSequence(dialogueData.stage3ClearDialogues[0].lines);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 버튼의 활성/비활성 상태에 맞게 인터랙션과 글자색 변경
    /// </summary>
    private void UpdateButton(Button button, bool unlocked)
    {
        if (button == null) return;

        // ✅ 잠겨 있어도 클릭은 되도록 유지 (onClick에서 TryStartStage가 처리함)
        button.interactable = true;

        // 비주얼만 잠김처럼 보이게
        var txt = button.GetComponentInChildren<TMP_Text>();
        if (txt) txt.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);

        // (선택) 잠긴 상태일 때 버튼 전환 효과를 죽여 '비활성 느낌' 주기
        if (!unlocked)
        {
            button.transition = Selectable.Transition.None;
        }
        else
        {
            button.transition = Selectable.Transition.ColorTint; // 프로젝트 기본에 맞게
        }
    }


    /// <summary>
    /// 현재 진행도에 맞춰 버튼 상태+색을 한 번에 반영
    /// </summary>
    private void ApplyButtonStatesWithColor()
    {
        UpdateButton(stage1Button, IsUnlocked(1)); // 항상 true(표기 일관성)
        UpdateButton(stage2Button, IsUnlocked(2)); // Stage1_Cleared==1이면 true
        UpdateButton(stage3Button, IsUnlocked(3)); // Stage2_Cleared==1이면 true
    }

}
