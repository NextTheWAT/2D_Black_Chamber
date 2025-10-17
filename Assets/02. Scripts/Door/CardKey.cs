using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardKey : MonoBehaviour, Iinteraction
{
    public static bool hasCardKey = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // ���ʱ�ȭ�� ����۶��� �ʱ�ȭ�ǰ�
    {
        hasCardKey = false;     // ī��Ű �ʱ�ȭ
    }

    public void Interaction(Transform interactor)
    {
        Debug.Log("ī��Ű�Ա�");
        hasCardKey = true;
        StructSoundManager.Instance.PlayPickUpSound(transform.position);
        gameObject.SetActive(false);
    }
}
