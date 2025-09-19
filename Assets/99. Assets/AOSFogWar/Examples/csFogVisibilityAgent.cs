using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FischlWorks_FogWar
{
    public class csFogVisibilityAgent : MonoBehaviour
    {
        [SerializeField] private csFogWar fogWar = null;

        [Header("Visibility (Spot/LOS)")]
        [Range(0, 2)][SerializeField] private int additionalRadius = 0;
        [Tooltip("Spot ����ġ �Ӱ谪(0..1). �� �� �̻��̸� ���̴� ������ ����")]
        [Range(0f, 1f)][SerializeField] private float spotThreshold = 0.01f;

        [Header("Fade Settings")]
        [Tooltip("���� ������� �ɸ��� �ð�(��)")]
        [SerializeField] private float fadeInTime = 0.12f;
        [Tooltip("���� ����� ������ �ɸ��� �ð�(��)")]
        [SerializeField] private float fadeOutTime = 0.35f;
        [Tooltip("alpha�� 0�� �Ǹ� �������� ���� ����(����/Ŭ�� ����)")]
        [SerializeField] private bool disableRendererAtZero = true;

        // cached renderers
        private List<MeshRenderer> meshRenderers;
        private List<SkinnedMeshRenderer> skinnedMeshRenderers;
        private List<SpriteRenderer> spriteRenderers;

        // remember original colors
        private List<Color> meshBaseColors;
        private List<Color> skinnedBaseColors;
        private List<Color> spriteBaseColors;

        // optional spot-check via reflection (to keep compatibility)
        private MethodInfo checkSpotMI;

        // fade state
        private float currentAlpha = 1f;
        private float targetAlpha = 1f;

        // material property block (to avoid material instancing)
        private MaterialPropertyBlock mpb;

        private void Start()
        {
            if (fogWar == null)
            {
                fogWar = FindObjectOfType<csFogWar>();
                if (fogWar == null)
                {
                    Debug.LogError("csFogVisibilityAgent: FogWar�� ã�� �� �����ϴ�. ���� csFogWar�� �־�� �մϴ�.");
                    enabled = false; return;
                }
            }

            // cache renderers (include inactive children)
            meshRenderers = GetComponentsInChildren<MeshRenderer>(true).ToList();
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true).ToList();
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();

            // base colors
            meshBaseColors = meshRenderers.Select(r => GetRendererBaseColor(r)).ToList();
            skinnedBaseColors = skinnedMeshRenderers.Select(r => GetRendererBaseColor(r)).ToList();
            spriteBaseColors = spriteRenderers.Select(r => r ? r.color : Color.white).ToList();

            // try to bind CheckVisibilitySpot(Vector3,int,float)
            checkSpotMI = typeof(csFogWar).GetMethod(
                "CheckVisibilitySpot",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(Vector3), typeof(int), typeof(float) },
                modifiers: null
            );

            mpb = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (fogWar == null) return;
            if (!fogWar.CheckWorldGridRange(transform.position)) return;

            bool visible;
            if (checkSpotMI != null)
            {
                object ret = checkSpotMI.Invoke(fogWar, new object[] { (Vector3)transform.position, additionalRadius, spotThreshold });
                visible = (ret is bool b) && b;
            }
            else
            {
                visible = fogWar.CheckVisibility(transform.position, additionalRadius);
            }

            // 1) ��ǥ ���� ����
            targetAlpha = visible ? 1f : 0f;

            // 2) ���� �ӵ�(�ʴ� ��ȭ��) ���
            float speed = visible
                ? (fadeInTime > 0f ? 1f / fadeInTime : 999f)
                : (fadeOutTime > 0f ? 1f / fadeOutTime : 999f);

            // 3) ���� ����
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, speed * Time.deltaTime);

            // 4) ����
            ApplyAlpha(currentAlpha);

            // 5) ������ �¿���(������ 0�� ���� ����)
            bool shouldEnable = currentAlpha > 0.001f || !disableRendererAtZero;
            SetRenderersEnabled(shouldEnable);
        }

        private void ApplyAlpha(float a01)
        {
            // Sprites
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var r = spriteRenderers[i];
                if (!r) continue;
                var baseC = (i < spriteBaseColors.Count) ? spriteBaseColors[i] : Color.white;
                r.color = new Color(baseC.r, baseC.g, baseC.b, baseC.a * a01);
            }

            // Mesh / SkinnedMesh
            // �켱����: _BaseColor(URP/HDRP) �� _Color(����/Ŀ����)
            for (int i = 0; i < meshRenderers.Count; i++)
                SetRendererAlpha(meshRenderers[i], (i < meshBaseColors.Count) ? meshBaseColors[i] : Color.white, a01);

            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
                SetRendererAlpha(skinnedMeshRenderers[i], (i < skinnedBaseColors.Count) ? skinnedBaseColors[i] : Color.white, a01);
        }

        private void SetRendererAlpha(Renderer r, Color baseColor, float a01)
        {
            if (!r) return;

            // ���� ��Ƽ������ �÷� ������Ƽ�� ���� ������ PropertyBlock���� ����
            r.GetPropertyBlock(mpb);
            bool hasBase = r.sharedMaterial && r.sharedMaterial.HasProperty("_BaseColor");
            bool hasColor = r.sharedMaterial && r.sharedMaterial.HasProperty("_Color");

            if (hasBase)
                mpb.SetColor("_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * a01));
            if (hasColor)
                mpb.SetColor("_Color", new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * a01));

            // �� �� ������ ����(�ش� ���̴��� ���� ������)
            r.SetPropertyBlock(mpb);
        }

        private Color GetRendererBaseColor(Renderer r)
        {
            if (!r || !r.sharedMaterial) return Color.white;
            if (r.sharedMaterial.HasProperty("_BaseColor")) return r.sharedMaterial.GetColor("_BaseColor");
            if (r.sharedMaterial.HasProperty("_Color")) return r.sharedMaterial.GetColor("_Color");
            return Color.white;
        }

        private void SetRenderersEnabled(bool enabled)
        {
            for (int i = 0; i < meshRenderers.Count; i++) if (meshRenderers[i]) meshRenderers[i].enabled = enabled;
            for (int i = 0; i < skinnedMeshRenderers.Count; i++) if (skinnedMeshRenderers[i]) skinnedMeshRenderers[i].enabled = enabled;
            for (int i = 0; i < spriteRenderers.Count; i++) if (spriteRenderers[i]) spriteRenderers[i].enabled = enabled;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fogWar == null || Application.isPlaying == false) return;

            bool visible;
            if (checkSpotMI != null)
            {
                object ret = checkSpotMI.Invoke(fogWar, new object[] { (Vector3)transform.position, additionalRadius, spotThreshold });
                visible = (ret is bool b) && b;
            }
            else
            {
                visible = fogWar.CheckVisibility(transform.position, additionalRadius);
            }

            float r = (fogWar._UnitScale * 0.5f) + additionalRadius;
#if UNITY_EDITOR
            Handles.color = visible ? Color.green : Color.yellow;
            Handles.DrawWireDisc(new Vector3(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                -0.1f), // camera front
                Vector3.forward, r);
#else
            Gizmos.color = visible ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, -0.1f), r);
#endif
        }
#endif
    }
}
