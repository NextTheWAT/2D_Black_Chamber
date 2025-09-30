using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField] public string clearSceneName = "ClearScene";  // 클리어씬

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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || mm == null) return;

        // 탈출 가능한 상태에서만 클리어 처리
        if (mm.Phase != MissionPhase.Escape)
        {
            Debug.Log("클리어 조건을 만족하지 않았습니다.");
            return;
        }

        Debug.Log("클리어하였습니다.");
        mm.SetPhase(MissionPhase.Completed);

        // 1) 잠입/난전 상태 텍스트
        bool isStealthClear = (GameManager.Instance != null && !GameManager.Instance.IsCombat);
        string clearStateText = isStealthClear ? "잠입 상태 클리어" : "난전 상태 클리어";

        // 2) 현재 스테이지 번호 추출 (예: "Stage1" → 1)
        string sceneName = SceneManager.GetActiveScene().name;
        int stageNumber = ExtractStageNumber(sceneName);

        // 3) 스테이지별 보상 데이터 조회
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
            Debug.LogWarning($"[ExitPortal] StageRewardManager 미배치 또는 스테이지 번호 추출 실패. scene='{sceneName}', num={stageNumber}");
        }

        // 4) 플레이어 소지금에 반영
        if (MoneyManager.Instance != null && reward > 0)
            MoneyManager.Instance.AddMoney(reward);

        // 5) 결과 데이터 생성(클리어 씬 UI에서 사용)
        if (GameStats.Instance != null)
            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

        // 6) 클리어 씬 로드
        SceneManager.LoadScene(clearSceneName);
    }

    // 씬 이름에서 숫자만 추출하여 스테이지 번호로 사용 (숫자 없으면 0)
    private int ExtractStageNumber(string sceneName)
    {
        string num = Regex.Replace(sceneName, "[^0-9]", "");
        return int.TryParse(num, out int v) ? v : 0;
    }
}
