using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // F키 입력 읽기용
using System.Collections;
using Esper.Freeloader; // ⬅️ 코루틴 사용을 위해 추가

static class ProgressFlags
{
    public static bool Get(string key) => PlayerPrefs.GetInt(key, 0) == 1;
    public static void Set(string key, bool v)
    {
        PlayerPrefs.SetInt(key, v ? 1 : 0);
        PlayerPrefs.Save();
    }

    public const string FirstMeetDone = "NPC_FirstMeetDone";       // 최초 대화 1회 처리용
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

    // ⬅️ 타이핑 설정 추가
    [Header("Typing Settings")]
    [SerializeField] private float typingDelay = 0.05f; // 글자당 딜레이 (0.05초)

    // TV 켜지는 효과 오브젝트들
    [Header("TV Effect Objects")]
    [SerializeField] private GameObject tvOnEffect1; // 첫 번째 TV 효과
    [SerializeField] private GameObject tvOnEffect2; // 두 번째 TV 효과
    [SerializeField] private GameObject tvOnEffect3; // 세 번째 TV 효과
    [SerializeField] private GameObject tvOnEffect4; // 네 번째 TV 효과
    [SerializeField] private GameObject tvOnEffect5; // 다섯 번째 TV 효과
    [SerializeField] private GameObject tvOnEffect6; // 여섯 번째 TV 효과

    // 추가: 각 효과 간의 지연 시간 (초)
    [SerializeField] private float effectDelay = 0.2f;

    private List<string> currentLines; // 현재 진행 중 "한 세트"의 줄들
    private int currentIndex = -1;     // 다음에 보여줄 줄 인덱스
    private bool inSequence = false;   // 대사 진행 중 여부
    private Coroutine typingCoroutine; // 타이핑 코루틴 제어용

    private bool _isEffectPlaying = false;
    private bool _hasOpenExecuted = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (_hasOpenExecuted) return;
        _hasOpenExecuted = true;

        // UI가 열리면, 실제 UI 내용을 보여주기 전에 TV 켜짐 효과 시퀀스 시작
        StartCoroutine(PlayTVOnEffectSequence());
    }

    // TV 켜짐 효과 6단계를 순차적으로 보여준 후, StageSelectDialogueUI 본체를 초기화하고 팝업
    private IEnumerator PlayTVOnEffectSequence()
    {
        _isEffectPlaying = true;

        // 1. 초기 상태: TVonEffect 오브젝트들 비활성화 (Unity Inspector에서 미리 설정해두는 것을 권장)
        if (tvOnEffect1) tvOnEffect1.SetActive(false);
        if (tvOnEffect2) tvOnEffect2.SetActive(false);
        if (tvOnEffect3) tvOnEffect3.SetActive(false);
        if (tvOnEffect4) tvOnEffect4.SetActive(false);
        if (tvOnEffect5) tvOnEffect5.SetActive(false);
        if (tvOnEffect6) tvOnEffect6.SetActive(false);

        // A. 첫 번째 효과 (TVonEffect1)
        if (tvOnEffect1)
        {
            tvOnEffect1.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect1.SetActive(false);
        }

        // B. 두 번째 효과 (TVonEffect2)
        if (tvOnEffect2)
        {
            tvOnEffect2.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect2.SetActive(false);
        }

        // C. 세 번째 효과 (TVonEffect3)
        if (tvOnEffect3)
        {
            tvOnEffect3.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect3.SetActive(false);
        }

        // D. 네 번째 효과 (TVonEffect4)
        if (tvOnEffect4)
        {
            tvOnEffect4.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect4.SetActive(false);
        }

        // E. 다섯 번째 효과 (TVonEffect5)
        if (tvOnEffect5)
        {
            tvOnEffect5.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay);
            tvOnEffect5.SetActive(false);
        }

        // F. 여섯 번째 효과 (TVonEffect6)
        if (tvOnEffect6)
        {
            tvOnEffect6.SetActive(true);
            yield return new WaitForSecondsRealtime(effectDelay * 1.5f);
            tvOnEffect6.SetActive(false);
        }

        _isEffectPlaying = false;

        // 2. 모든 효과가 끝나면 원래의 OnOpen 로직 실행 (UI 표시 시작)
        ExecuteOnOpenLogic();
    }

    private void ExecuteOnOpenLogic()
    {
        Time.timeScale = 0f; // 일시정지 (TV 효과가 끝난 후 정지)
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

        // NPC 이름 표시
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
        // 1. TV 효과 재생 중이거나, 2. 대사 시퀀스 진행 중이 아닐 때는 F키 입력 무시
        if (_isEffectPlaying || !inSequence) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (typingCoroutine != null)
            {
                // 1. 타이핑 중이었다면, 타이핑을 멈추고 현재 줄을 즉시 완료
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (lineText && currentLines != null && currentIndex < currentLines.Count)
                {
                    lineText.text = currentLines[currentIndex];
                }
            }
            else
            {
                // 2. 타이핑이 완료되었다면, 다음 줄로 넘어감
                NextLine();
            }
        }
    }

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

        NextLine(); // 첫 줄 즉시 출력 (이 안에서 타이핑 시작)
    }

    private void NextLine()
    {
        currentIndex++;

        if (currentLines == null || currentIndex >= currentLines.Count)
        {
            EndSequence();
            return;
        }

        // 기존 즉시 출력 대신 타이핑 코루틴 시작
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (lineText) lineText.text = ""; // 새로운 줄 시작 시 텍스트 초기화

        typingCoroutine = StartCoroutine(TypeSentence(currentLines[currentIndex]));
    }

    /// 글자를 한 글자씩 출력하는 코루틴 (타이핑 애니메이션)
    private IEnumerator TypeSentence(string sentence)
    {
        // TimeScale이 0이므로 WaitForSecondsRealtime을 사용해야 멈추지 않고 재생됨
        foreach (char letter in sentence.ToCharArray())
        {
            lineText.text += letter;
            yield return new WaitForSecondsRealtime(typingDelay);
        }

        // 타이핑이 완전히 끝나면 Coroutine 참조를 해제하여
        // Update()에서 F키 입력 시 NextLine()이 호출되도록 준비합니다.
        typingCoroutine = null;
    }


    private void EndSequence()
    {
        inSequence = false;
        currentLines = null;
        currentIndex = -1;

        // 타이핑 코루틴이 남아있을 경우 정지
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

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
        // (생략된 기존 TryStartStage 로직)
        UISoundManager.Instance.PlayButtonClickSound(Vector2.zero);

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
                // 처음 재생하는 경우, 영구 플래그를 설정하여 다시는 재생되지 않도록함
                ProgressFlags.Set(ProgressFlags.ClearDialogueDone(1), true);

                if (dialogueData?.stage1ClearDialogues != null &&
                  dialogueData.stage1ClearDialogues.Count > 0 &&
                  dialogueData.stage1ClearDialogues[0].lines?.Count > 0)
                {
                    StartSequence(dialogueData.stage1ClearDialogues[0].lines);
                    return true;
                }
            }
        }
        // 2스테이지 클리어 예약
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
                    StartSequence(dialogueData.stage2ClearDialogues[0].lines);
                    return true;
                }
            }
        }

        // 3스테이지 클리어 예약
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
                    StartSequence(dialogueData.stage3ClearDialogues[0].lines);
                    return true;
                }
            }
        }

        // 4스테이지 클리어 예약
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
                    StartSequence(dialogueData.stage4ClearDialogues[0].lines);
                    return true;
                }
            }
        }

        // 5스테이지 클리어 예약
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
                    StartSequence(dialogueData.stage5ClearDialogues[0].lines);
                    return true;
                }
            }
        }

        return false;
    }

    // 버튼의 활성/비활성 상태에 맞게 인터랙션과 글자색 변경
    private void UpdateButton(Button button, bool unlocked)
    {
        if (button == null) return;

        //  잠겨 있어도 클릭은 되도록 유지 (onClick에서 TryStartStage가 처리함)
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

    // 현재 진행도에 맞춰 버튼 상태+색을 한 번에 반영
    private void ApplyButtonStatesWithColor()
    {
        UpdateButton(stage1Button, IsUnlocked(1)); // 항상 true(표기 일관성)
        UpdateButton(stage2Button, IsUnlocked(2)); // Stage1_Cleared==1이면 true
        UpdateButton(stage3Button, IsUnlocked(3)); // Stage2_Cleared==1이면 true
        UpdateButton(stage4Button, IsUnlocked(4)); // Stage3_Cleared==1이면 true
        UpdateButton(stage5Button, IsUnlocked(5));
    }
}