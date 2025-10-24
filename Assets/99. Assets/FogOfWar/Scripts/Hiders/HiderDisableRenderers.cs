using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{
    // HiderDisableRenderers 대신 HiderFadeRenderers로 이름 변경을 권장합니다.
    public class HiderFadeRenderers : HiderBehavior
    {
        [SerializeField] private Renderer[] ObjectsToHide;
        // 페이드 효과에 걸릴 시간 (초)
        [SerializeField] private float fadeDuration = 0.5f;

        private Coroutine currentFadeCoroutine;

        // 페이드 인/아웃 코루틴을 시작하고 이전 코루틴을 중지합니다.
        private void StartFade(float targetAlpha)
        {
            if (currentFadeCoroutine != null)
                StopCoroutine(currentFadeCoroutine);

            currentFadeCoroutine = StartCoroutine(FadeCoroutine(targetAlpha));
        }

        // 'OnHide'는 '0'으로 페이드 아웃을 시작합니다.
        protected override void OnHide()
        {
            StartFade(0f);
        }

        // 'OnReveal'은 '1'로 페이드 인을 시작합니다.
        protected override void OnReveal()
        {
            StartFade(1f);
        }

        // 알파 값을 부드럽게 변화시키는 코루틴
        private IEnumerator FadeCoroutine(float targetAlpha)
        {
            float time = 0;

            // 숨기려는 오브젝트가 Material을 가지고 있어야 합니다.
            foreach (Renderer renderer in ObjectsToHide)
            {
                if (renderer.material.HasProperty("_Color")) // 머티리얼이 'Color' 속성을 가지고 있는지 확인 (대부분의 Standard Shader에 있음)
                {
                    Color startColor = renderer.material.color;
                    Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

                    while (time < fadeDuration)
                    {
                        // Lerp를 사용하여 시간 기반으로 부드럽게 색상(알파)을 변화시킵니다.
                        renderer.material.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
                        time += Time.deltaTime;
                        yield return null; // 다음 프레임까지 대기
                    }

                    // 최종적으로 목표 알파 값으로 설정하여 정확도를 보장합니다.
                    renderer.material.color = targetColor;
                }
            }

            currentFadeCoroutine = null;
        }


        // 새로운 배열로 교체 시, 현재 상태에 맞춰 페이드 인/아웃을 시작합니다.
        public void ModifyHiddenRenderers(Renderer[] newObjectsToHide)
        {
            // 새롭게 숨기려는 오브젝트들을 표시(1로 페이드)합니다.
            // 이후 새로운 배열로 교체하고, 현재 상태에 맞게 Hide 또는 Reveal을 호출합니다.

            // 이전 오브젝트 배열이 있다면, 모두 표시(Reveal)합니다.
            if (ObjectsToHide != null)
            {
                // 이전 오브젝트들을 표시
                if (currentFadeCoroutine != null)
                    StopCoroutine(currentFadeCoroutine);

                // 기존 오브젝트들은 즉시 나타나게 하거나, 별도의 FadeOut 로직 필요 (여기서는 간단하게 즉시 OnReveal 효과를 주는 로직 사용)
                SetAlphaInstant(1f); // 이전 오브젝트들을 즉시 보이게 합니다.
            }

            // 새로운 오브젝트 배열로 교체
            ObjectsToHide = newObjectsToHide;

            // 컴포넌트가 활성화되지 않았다면 작동하지 않습니다.
            if (!enabled)
                return;

            // 현재 FOW 시스템 상태에 따라 페이드 인 또는 페이드 아웃을 시작합니다.
            if (!IsEnabled)
                OnHide(); // 숨김 상태라면 페이드 아웃
            else
                OnReveal(); // 노출 상태라면 페이드 인 (이미 보일 테지만, 코루틴 시작을 막기 위해 조건 추가 고려 필요)
        }

        // 페이드 로직이 아닌, 즉시 알파 값 설정을 위한 헬퍼 함수
        private void SetAlphaInstant(float alpha)
        {
            if (ObjectsToHide == null) return;
            foreach (Renderer renderer in ObjectsToHide)
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    Color color = renderer.material.color;
                    renderer.material.color = new Color(color.r, color.g, color.b, alpha);
                }
            }
        }
    }
}