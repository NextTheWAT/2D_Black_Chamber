using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.Linq;
#if UNITY_EDITOR

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonShadowCaster2DCreator : MonoBehaviour
{
    [SerializeField]
    private bool selfShadows = true;

    private PolygonCollider2D polygonCollider;

    static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                                                    .Assembly
                                                                    .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    public void Create()
    {
        DestroyOldShadowCasters();
        polygonCollider = GetComponent<PolygonCollider2D>();

        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            Vector2[] pathVertices = polygonCollider.GetPath(i);
            GameObject shadowCaster = new("shadow_caster_" + i);
            Debug.Log("Creating shadow caster " + i + " with " + pathVertices.Length + " vertices");
            shadowCaster.transform.position = transform.position;
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];

            for (int j = 0; j < pathVertices.Length; j++)
                testPath[j] = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * pathVertices[j];

            shapePathField.SetValue(shadowCasterComponent, testPath);
            shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue));
            meshField.SetValue(shadowCasterComponent, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterComponent,
            new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
        }
    }

    public void DestroyOldShadowCasters()
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            if(child.TryGetComponent<ShadowCaster2D>(out var sc))
                DestroyImmediate(child.gameObject);
        }
    }
}


[CustomEditor(typeof(PolygonShadowCaster2DCreator))]
public class PolygonShadowCaster2DCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create"))
        {
            var creator = (PolygonShadowCaster2DCreator)target;
            creator.Create();
        }

        if (GUILayout.Button("Remove Shadows"))
        {
            var creator = (PolygonShadowCaster2DCreator)target;
            creator.DestroyOldShadowCasters();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
