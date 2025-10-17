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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬초기화와 재시작때도 초기화되게
    {
        hasCardKey = false;     // 카드키 초기화
    }

    public void Interaction(Transform interactor)
    {
        Debug.Log("카드키먹기");
        hasCardKey = true;
        StructSoundManager.Instance.PlayPickUpSound(transform.position);
        gameObject.SetActive(false);
    }
}
