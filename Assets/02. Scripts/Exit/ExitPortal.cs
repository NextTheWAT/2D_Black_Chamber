using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;
    public string clearSceneName = "";      // 클리어씬
    private bool npcKilled = false;

    private void Start()
    {
        defaultLight.SetActive(true);
        clearLight.SetActive(false);
    }
    public void isNpcKilled()
    {
        Debug.Log("클리어 조건을 만족하였습니다.");
        npcKilled = true;

        defaultLight.SetActive(false);
        clearLight.SetActive(true);
        Debug.Log("탈출 가능");
    }

    // 연구원이 맵에 없을때 열리게

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && npcKilled)  // npc태그? 연구원만 적용
        {
            Debug.Log("클리어하였습니다.");
            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("클리어 조건을 만족하지 않았습니다.");
        }
    }
}
