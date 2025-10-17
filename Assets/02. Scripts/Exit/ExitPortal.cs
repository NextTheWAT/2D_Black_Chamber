using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    [Header("Visual")]
    public GameObject defaultLight;
    public GameObject clearLight;

    public Door exitDoor;

    [Header("Scene")]
    [SerializeField] public string clearSceneName = "ClearScene";  // 클리어씬
    [SerializeField] private int stageNumber = 1;

    [Header("Tutorial Exit")]
    [SerializeField] private bool isTutorialExit = false;
    [SerializeField] private string loobySceneName = "LobbyScene"; //튜토리얼 완료 후 이동할 씬
    private const string PrefKey_TutorialDone = "TutorialDone";

    private MissionManager mm;

    private void Start()
    {
        // 초기 조명 상태
        if (defaultLight) defaultLight.SetActive(true);
        if (clearLight) clearLight.SetActive(false);

        // 미션 단계 이벤트 구독
        mm = MissionManager.Instance;
        if (mm != null) mm.OnPhaseChanged += ExitPhaseChanged;
    }

    private void OnDestroy()
    {
        if (mm != null) mm.OnPhaseChanged -= ExitPhaseChanged;
    }

    private void ExitPhaseChanged(MissionPhase phase)
    {
        // 암살 대상 제거 완료 → 탈출 가능 단계로 바뀌면 조명 전환
        if (phase == MissionPhase.Escape)
        {
            if (defaultLight) defaultLight.SetActive(false);
            if (clearLight) clearLight.SetActive(true);
            Debug.Log("탈출 가능");

            if (exitDoor != null)
                exitDoor.AutoOpen();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (mm == null) mm = MissionManager.Instance;
        if (mm == null)
        {
            Debug.LogWarning("[ExitPortal] MissionManager가 없습니다.");
            return;
        }

        if (mm.Phase != MissionPhase.Escape)
        {
            Debug.Log("클리어 조건을 만족하지 않았습니다. (아직 목표물이 남아있음)");
            return;
        }

        if (isTutorialExit)
        {
            // 튜토리얼 출구: 클리어 씬을 건너뛰고 로비로 이동
            PlayerPrefs.SetInt(PrefKey_TutorialDone, 1);
            GA.Tutorial_Complete();
            PlayerPrefs.Save();
            SceneManager.LoadScene(loobySceneName);
            return;
        }

        var tracker = FindAnyObjectByType<StageRunTracker>();
        if (tracker != null)
        {
            tracker.ReportComplete();
        }


        Debug.Log("클리어하였습니다.");
        mm.SetPhase(MissionPhase.Completed);

        // 1) 잠입/난전 상태 텍스트
        bool isStealthClear = (GameManager.Instance != null && !GameManager.Instance.IsCombat);
        string clearStateText = isStealthClear ? "잠입 상태 클리어" : "난전 상태 클리어";

        // 2) 스테이지별 보상 데이터 조회
        int reward = 0;
        var srm = StageRewardManager.Instance;
        if (srm != null && stageNumber > 0)
        {
            StageReward data = srm.GetReward(stageNumber);
            if (data != null)
            {
                reward = isStealthClear ? data.stealthReward : data.combatReward;
            }
            else
            {
                Debug.LogWarning($"[ExitPortal] 보상 데이터 없음: stage={stageNumber}");
            }
        }
        else
        {
            Debug.LogWarning("[ExitPortal] StageRewardManager 없음 또는 stageNumber 잘못 설정됨");
        }

        // 3) 플레이어 소지금에 반영
        if (MoneyManager.Instance != null && reward > 0)
            MoneyManager.Instance.AddMoney(reward);

        // 4) 결과 데이터 생성(클리어 씬 UI에서 사용)
        if (GameStats.Instance != null)
            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

        // 5) 클리어 씬 로드
        SceneManager.LoadScene(clearSceneName);
    }
}
