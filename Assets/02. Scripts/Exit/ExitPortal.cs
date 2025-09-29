using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField] public string clearSceneName = "ClearScene";  // Ŭ�����

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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || mm == null) return;

        // Ż�� ������ ���¿����� Ŭ���� ó��
        if (mm.Phase != MissionPhase.Escape)
        {
            Debug.Log("Ŭ���� ������ �������� �ʾҽ��ϴ�.");
            return;
        }

        Debug.Log("Ŭ�����Ͽ����ϴ�.");
        mm.SetPhase(MissionPhase.Completed);

        // 1) ����/���� ���� �ؽ�Ʈ
        bool isStealthClear = (GameManager.Instance != null && !GameManager.Instance.IsCombat);
        string clearStateText = isStealthClear ? "���� ���� Ŭ����" : "���� ���� Ŭ����";

        // 2) ���� �������� ��ȣ ���� (��: "Stage1" �� 1)
        string sceneName = SceneManager.GetActiveScene().name;
        int stageNumber = ExtractStageNumber(sceneName);

        // 3) ���������� ���� ������ ��ȸ
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
            Debug.LogWarning($"[ExitPortal] StageRewardManager �̹�ġ �Ǵ� �������� ��ȣ ���� ����. scene='{sceneName}', num={stageNumber}");
        }

        // 4) �÷��̾� �����ݿ� �ݿ�
        if (MoneyManager.Instance != null && reward > 0)
            MoneyManager.Instance.AddMoney(reward);

        // 5) ��� ������ ����(Ŭ���� �� UI���� ���)
        if (GameStats.Instance != null)
            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

        // 6) Ŭ���� �� �ε�
        SceneManager.LoadScene(clearSceneName);
    }

    // �� �̸����� ���ڸ� �����Ͽ� �������� ��ȣ�� ��� (���� ������ 0)
    private int ExtractStageNumber(string sceneName)
    {
        string num = Regex.Replace(sceneName, "[^0-9]", "");
        return int.TryParse(num, out int v) ? v : 0;
    }
}
