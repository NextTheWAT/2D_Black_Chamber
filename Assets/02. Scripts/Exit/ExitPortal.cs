using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public string clearSceneName = "";      // Ŭ�����
    private bool npcKilled = false;

    public void isNpcKilled()
    {
        Debug.Log("Ŭ���� ������ �����Ͽ����ϴ�.");
        npcKilled = true;
    }

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
