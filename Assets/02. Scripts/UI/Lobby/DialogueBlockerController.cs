using UnityEngine;
using UnityEngine.UI;

public class DialogueBlockerController : MonoBehaviour
{
    private Image blockerImage;
    private bool isInitialized = false;

    void Awake()
    {
        // Awake가 OnOpen보다 먼저 실행되도록 순서를 설정했다면, 
        // 여기서 초기화가 완료됩니다.
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (isInitialized) return;

        blockerImage = GetComponent<Image>();
        if (blockerImage == null)
        {
            // 이 로그가 다시 뜨면 Panel에 Image 컴포넌트가 없거나 다른 문제
            Debug.LogError("DialogueBlockerController: Image 컴포넌트를 찾을 수 없어 블로커 작동 불가.", this);
            return;
        }

        // 초기 상태: 비활성화
        blockerImage.gameObject.SetActive(false);
        isInitialized = true; // 초기화 성공
    }

    public void SetBlockerActive(bool isActive)
    {
        // 스크립트 실행 순서를 설정했더라도, 만약을 대비해 강제 초기화 시도
        if (!isInitialized)
        {
            TryInitialize();
        }

        if (blockerImage != null)
        {
            blockerImage.gameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogError("SetBlockerActive 호출 실패: blockerImage가 여전히 null입니다. Image 컴포넌트 확인 필요.", this);
        }
    }
}