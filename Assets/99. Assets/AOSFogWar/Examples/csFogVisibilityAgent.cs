/*
 * 2D Top-Down Fog Visibility Agent (Spot-ready)
 * - Uses FogWar.CheckVisibilitySpot(worldPos, additionalRadius, threshold) when available
 * - Falls back to CheckVisibility(...) if Spot check is missing
 * - Toggles MeshRenderer, SkinnedMeshRenderer, SpriteRenderer
 * - Draws gizmo on XY plane
 */

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

        // cached renderers
        private List<MeshRenderer> meshRenderers;
        private List<SkinnedMeshRenderer> skinnedMeshRenderers;
        private List<SpriteRenderer> spriteRenderers;

        // optional spot-check via reflection (to keep compatibility)
        private MethodInfo checkSpotMI;

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

            // try to bind CheckVisibilitySpot(Vector3,int,float)
            checkSpotMI = typeof(csFogWar).GetMethod(
                "CheckVisibilitySpot",
                BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: new[] { typeof(Vector3), typeof(int), typeof(float) },
                modifiers: null
            );
        }

        private void Update()
        {
            if (fogWar == null) return;
            if (!fogWar.CheckWorldGridRange(transform.position)) return;

            bool visible;

            if (checkSpotMI != null)
            {
                // Spot(��ä��) ����
                object ret = checkSpotMI.Invoke(fogWar, new object[] { (Vector3)transform.position, additionalRadius, spotThreshold });
                visible = (ret is bool b) && b;
            }
            else
            {
                // ����: ����(LOS) ����
                visible = fogWar.CheckVisibility(transform.position, additionalRadius);
            }

            // toggle all renderers
            for (int i = 0; i < meshRenderers.Count; i++) if (meshRenderers[i]) meshRenderers[i].enabled = visible;
            for (int i = 0; i < skinnedMeshRenderers.Count; i++) if (skinnedMeshRenderers[i]) skinnedMeshRenderers[i].enabled = visible;
            for (int i = 0; i < spriteRenderers.Count; i++) if (spriteRenderers[i]) spriteRenderers[i].enabled = visible;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fogWar == null || Application.isPlaying == false) return;

            // do the same check used in Update()
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

            // XY plane gizmo (disc)
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
