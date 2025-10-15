using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    public Door exitDoor;

    [SerializeField] public string clearSceneName = "ClearScene";  // Ŭ�����
    [SerializeField] private int stageNumber = 1;

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
            {
                exitDoor.AutoOpen();
            }
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
