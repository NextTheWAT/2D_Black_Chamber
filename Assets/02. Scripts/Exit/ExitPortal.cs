using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField]
    public string clearSceneName = "ClearScene";      // 클리어씬

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
            Debug.Log("탈출 가능");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || mm == null) return;

        if (mm.Phase == MissionPhase.Escape)
        {
            Debug.Log("클리어하였습니다.");
            mm.SetPhase(MissionPhase.Completed);

            string clearStateText = (GameManager.Instance != null && !GameManager.Instance.IsCombat)
                ? "잠입 상태 클리어" : "난전 상태 클리어";

            int reward = 1500; //보상 임시

            TempResultHolder.Data = GameStats.Instance.BuildClearResult(clearStateText, reward);

            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("클리어 조건을 만족하지 않았습니다.");
        }
    }
}
