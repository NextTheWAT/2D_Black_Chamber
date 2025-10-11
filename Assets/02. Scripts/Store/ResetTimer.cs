using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResetTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    public float resetInterval = 43200f;    // 12시간
    private float time;

    private void Start()
    {
        time = resetInterval;
        UpdateTimerUI();
        StartCoroutine(TimerCoroutine());
    }
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            time -= 1f;

            if (time <= 0f)
            {
                time = resetInterval;
                ResetItem();
            }

            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int hours = Mathf.FloorToInt(time / 3600);  // FloorToInt = 소수점이하 버리기
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    private void ResetItem()
    {
        // 아이템 갱신되게
    }
}
