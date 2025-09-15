using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField]
    public string clearSceneName = "";      // Ŭ�����

    private void Start()
    {
        defaultLight.SetActive(true);
        clearLight.SetActive(false);

        MissionManager.Instance.OnPhaseChanged += ExitPhaseChanged; // ������ü�����϶� Exit������ü������ ����
    }

    private void OnDestroy()
    {
        if(MissionManager.Instance != null)
            MissionManager.Instance.OnPhaseChanged -= ExitPhaseChanged; // �ı��ɶ� Exit������ü������ �ı�
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
        if (other.CompareTag("Player") && MissionManager.Instance.Phase == MissionPhase.Escape)
        {
            Debug.Log("Ŭ�����Ͽ����ϴ�.");

            MissionManager.Instance.SetPhase(MissionPhase.Completed);   // ������ Ŭ����� �ٲ��ֱ�

            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("Ŭ���� ������ �������� �ʾҽ��ϴ�.");
        }
    }
}
