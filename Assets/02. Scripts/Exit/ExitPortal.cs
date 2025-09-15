using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public GameObject defaultLight;
    public GameObject clearLight;
    public string clearSceneName = "";      // Ŭ�����
    private bool npcKilled = false;

    private void Start()
    {
        defaultLight.SetActive(true);
        clearLight.SetActive(false);
    }
    public void isNpcKilled()
    {
        Debug.Log("Ŭ���� ������ �����Ͽ����ϴ�.");
        npcKilled = true;

        defaultLight.SetActive(false);
        clearLight.SetActive(true);
        Debug.Log("Ż�� ����");
    }

    // �������� �ʿ� ������ ������

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && npcKilled)  // npc�±�? �������� ����
        {
            Debug.Log("Ŭ�����Ͽ����ϴ�.");
            SceneManager.LoadScene(clearSceneName);
        }
        else
        {
            Debug.Log("Ŭ���� ������ �������� �ʾҽ��ϴ�.");
        }
    }
}
