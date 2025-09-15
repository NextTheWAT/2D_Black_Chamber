using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;

    [SerializeField]
    public string clearSceneName = "";      // 클리어씬

    private void Start()
    {
        defaultLight.SetActive(true);
        clearLight.SetActive(false);

        MissionManager.Instance.OnPhaseChanged += ExitPhaseChanged; // 페이즈체인지일때 Exit페이즈체인지도 적용
    }

    private void OnDestroy()
    {
        if(MissionManager.Instance != null)
            MissionManager.Instance.OnPhaseChanged -= ExitPhaseChanged; // 파괴될때 Exit페이즈체인지도 파괴
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
        if (other.CompareTag("Player") && MissionManager.Instance.Phase == MissionPhase.Escape)
        {
            Debug.Log("클리어하였습니다.");

            MissionManager.Instance.SetPhase(MissionPhase.Completed);   // 페이즈 클리어로 바꿔주기

            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("클리어 조건을 만족하지 않았습니다.");
        }
    }
}
