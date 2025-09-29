using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField]
    public string clearSceneName = "ClearScene";      // Ŭ�����

    private MissionManager mm;

    private void Start()
    {
        defaultLight.SetActive(true);
        clearLight.SetActive(false);

        mm = MissionManager.Instance;
        if (mm != null)
            mm.OnPhaseChanged += ExitPhaseChanged;
    }

    private void OnDestroy()
    {
        if (mm != null)
            mm.OnPhaseChanged -= ExitPhaseChanged;
    }

    private void ExitPhaseChanged(MissionPhase phase)
    {
        if (phase == MissionPhase.Escape)
        {
            if (defaultLight != null)
                defaultLight.SetActive(false);
            if (clearLight != null)
                clearLight.SetActive(true);
            Debug.Log("Ż�� ����");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || mm == null) return;

        if (mm.Phase == MissionPhase.Escape)
        {
            Debug.Log("Ŭ�����Ͽ����ϴ�.");
            mm.SetPhase(MissionPhase.Completed);

            string clearStateText = (GameManager.Instance != null && !GameManager.Instance.IsCombat)
                ? "���� ���� Ŭ����" : "���� ���� Ŭ����";

            int reward = 1500; //���� �ӽ�

            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("Ŭ���� ������ �������� �ʾҽ��ϴ�.");
        }
    }
}
