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
    [SerializeField] public string clearSceneName = "ClearScene";  // Ŭ�����
    [SerializeField] private int stageNumber = 1;

    [Header("Tutorial Exit")]
    [SerializeField] private bool isTutorialExit = false;
    [SerializeField] private string loobySceneName = "LobbyScene"; //Ʃ�丮�� �Ϸ� �� �̵��� ��
    private const string PrefKey_TutorialDone = "TutorialDone";

    private MissionManager mm;

    private void Start()
    {
        // �ʱ� ���� ����
        if (defaultLight) defaultLight.SetActive(true);
        if (clearLight) clearLight.SetActive(false);

        // �̼� �ܰ� �̺�Ʈ ����
        mm = MissionManager.Instance;
        if (mm != null) mm.OnPhaseChanged += ExitPhaseChanged;
    }

    private void OnDestroy()
    {
        if (mm != null) mm.OnPhaseChanged -= ExitPhaseChanged;
    }

    private void ExitPhaseChanged(MissionPhase phase)
    {
        // �ϻ� ��� ���� �Ϸ� �� Ż�� ���� �ܰ�� �ٲ�� ���� ��ȯ
        if (phase == MissionPhase.Escape)
        {
            if (defaultLight) defaultLight.SetActive(false);
            if (clearLight) clearLight.SetActive(true);
            Debug.Log("Ż�� ����");

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
            Debug.LogWarning("[ExitPortal] MissionManager�� �����ϴ�.");
            return;
        }

        if (mm.Phase != MissionPhase.Escape)
        {
            Debug.Log("Ŭ���� ������ �������� �ʾҽ��ϴ�. (���� ��ǥ���� ��������)");
            return;
        }

        if (isTutorialExit)
        {
            // Ʃ�丮�� �ⱸ: Ŭ���� ���� �ǳʶٰ� �κ�� �̵�
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


        Debug.Log("Ŭ�����Ͽ����ϴ�.");
        mm.SetPhase(MissionPhase.Completed);

        // 1) ����/���� ���� �ؽ�Ʈ
        bool isStealthClear = (GameManager.Instance != null && !GameManager.Instance.IsCombat);
        string clearStateText = isStealthClear ? "���� ���� Ŭ����" : "���� ���� Ŭ����";

        // 2) ���������� ���� ������ ��ȸ
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
                Debug.LogWarning($"[ExitPortal] ���� ������ ����: stage={stageNumber}");
            }
        }
        else
        {
            Debug.LogWarning("[ExitPortal] StageRewardManager ���� �Ǵ� stageNumber �߸� ������");
        }

        // 3) �÷��̾� �����ݿ� �ݿ�
        if (MoneyManager.Instance != null && reward > 0)
            MoneyManager.Instance.AddMoney(reward);

        // 4) ��� ������ ����(Ŭ���� �� UI���� ���)
        if (GameStats.Instance != null)
            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

        // 5) Ŭ���� �� �ε�
        SceneManager.LoadScene(clearSceneName);
    }
}
