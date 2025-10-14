using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FischlWorks_FogWar;

// NOTE: This file consolidates Fog system + shape system.
// - Removes 3D remnants (fogPlaneHeight, ray* heights)
// - Fixes odd-height Y 0.5 offset
// - Fixes revealer world-pos double offset
// - Fixes "update only on move" gate (lastSeenAt update)
// - Unifies material usage & adds GPU-lerp (shader) with CPU fallback
// - Adds IRevealerShape-based custom shapes (Circle, Sector, TextureMask)

[DisallowMultipleComponent]
public class csFogWar : MonoBehaviour
{
    #region === Nested: Data ===
    [Serializable]
    public class LevelColumn
    {
        public enum ETileState { Empty, Obstacle }
        public List<ETileState> levelColumn = new List<ETileState>();

        public LevelColumn() { }
        public LevelColumn(IEnumerable<ETileState> init) => levelColumn = new List<ETileState>(init);
        public ETileState this[int y]
        {
            get => levelColumn[y];
            set => levelColumn[y] = value;
        }
    }

    [Serializable]
    public class LevelData
    {
        public int levelDimensionX;
        public int levelDimensionY;
        public float unitScale = 1f;
        public int scanSpacingPerUnit = 1;

        public List<LevelColumn> levelRow = new List<LevelColumn>();

        public void AddColumn(LevelColumn col) => levelRow.Add(col);
        public LevelColumn this[int x] => levelRow[x];
    }
    #endregion

    #region === Nested: Revealer ===
    [Serializable]
    public class FogRevealer
    {
        [Header("Transform")]
        [SerializeField] private Transform revealerTransform = null;
        public Transform _RevealerTransform => revealerTransform;

        [Tooltip("If true, 'forward' is transform.right; else transform.up.")]
        public bool forwardIsRight = true;

        [Header("Update Gate")]
        [SerializeField] private bool updateOnlyOnMove = true;
        public bool _UpdateOnlyOnMove => updateOnlyOnMove;

        [Header("Legacy (Fan) Shape - kept for backward compat")]
        public float outerRadius = 7f;   // units
        public float innerRadius = 3f;   // units
        public float outerAngleDeg = 90f;
        public float innerAngleDeg = 60f;

        [Header("New Shape Asset (recommended)")]
        public RevealerShapeAsset shapeAsset;          // Circle/Sector/TextureMask etc.
        public Vector2 shapeOffset = Vector2.zero;     // Local offset inside revealer
        public Vector2 shapeScale = Vector2.one;       // Non-uniform scale
        public float shapeRotationDeg = 0f;            // Local rotation override (added to Transform rotation)

        // cache for movement gate
        [SerializeField] private Vector2Int lastSeenAt = new Vector2Int(int.MaxValue, int.MaxValue);

        public Vector2 GetForward2D()
        {
            if (revealerTransform == null) return Vector2.right;
            var v = forwardIsRight ? (Vector2)revealerTransform.right : (Vector2)revealerTransform.up;
            if (v.sqrMagnitude < 1e-5f) v = Vector2.right;
            return v.normalized;
        }

        public Vector2Int GetCurrentLevelCoordinates(csFogWar fw)
        {
            if (revealerTransform == null) return lastSeenAt;
            return new Vector2Int(fw.GetUnitX(revealerTransform.position.x), fw.GetUnitY(revealerTransform.position.y));
        }

        /// <summary>
        /// Returns true if moved since last check AND updates lastSeenAt.
        /// </summary>
        public bool MovedSinceLastCheck(csFogWar fw)
        {
            var cur = GetCurrentLevelCoordinates(fw);
            if (cur != lastSeenAt) { lastSeenAt = cur; return true; }
            return false;
        }
    }
    #endregion

    #region === Serialized: Basic ===
    [Header("Basic Properties")]
    [SerializeField] private List<FogRevealer> fogRevealers = new List<FogRevealer>();
    public List<FogRevealer> Revealers => fogRevealers;

    [SerializeField] private Transform levelMidPoint = null; // center of grid in world
    [SerializeField][Range(1, 30)] private float FogRefreshRate = 15f; // field recompute Hz

    [Header("Fog Plane & Lerp")]
    [SerializeField] private Material fogPlaneMaterial = null; // Unlit transparent
    [SerializeField] private Color fogColor = new Color32(5, 15, 25, 255);
    [SerializeField][Range(0, 1)] private float fogPlaneAlpha = 1f;     // base fog opacity
    [SerializeField][Range(1, 5)] private float fogLerpSpeed = 2.5f;    // visual smoothing
    [SerializeField] public bool keepRevealedTiles = false;
    [SerializeField][Range(0, 1)] public float revealedTileOpacity = 0.5f;

    [Header("Level Data")]
    [SerializeField] private TextAsset LevelDataToLoad = null; // optional JSON
    [SerializeField] private bool saveDataOnScan = true;
    [SerializeField] private string levelNameToSave = "Default";

    [Header("Scan Properties")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private bool ignoreTriggers = true;
    [SerializeField] private int scanSpacingPerUnit = 1; // tiles per unit along X/Y

    [Header("Door Occlusion (raycast)")]
    [SerializeField] private LayerMask doorLayers; // blocks vision but not path grid

    [Header("Grid")]
    [SerializeField] private int levelDimensionX = 64;
    [SerializeField] private int levelDimensionY = 64;
    [SerializeField] private float unitScale = 1f; // world units per tile

    [Header("Debug Options")]
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private bool LogOutOfRange = false;
    #endregion

    #region === Runtime fields ===
    public LevelData levelData { get; private set; } = new LevelData();
    public Shadowcaster shadowcaster { get; private set; } = new Shadowcaster(); // external module already in your project

    private GameObject fogPlane;
    private Texture2D fogPlaneTextureLerpTarget; // latest computed
    private Texture2D fogPlaneTextureLerpBuffer; // previous shown (for CPU or initial GPU)
    private float[,] spotWeight;                 // 0..1 visibility accumulation

    private float refreshTimer;
    private float gpuLerpT;                      // GPU lerp progress 0..1
    private bool useGpuLerp;                     // true if material supports _FogTargetTex

    private static readonly int ID_Color = Shader.PropertyToID("_Color");
    private static readonly int ID_Target = Shader.PropertyToID("_FogTargetTex");
    private static readonly int ID_Buffer = Shader.PropertyToID("_FogBufferTex");
    private static readonly int ID_LerpT = Shader.PropertyToID("_LerpT");
    #endregion

    #region === Unity ===
    private void Start()
    {
        ValidateProperties();
        InitializeVariables();

        if (LevelDataToLoad == null) ScanLevel(); else LoadLevelData();
        InitializeFog();

        shadowcaster.Initialize(this);
        ForceUpdateFog();
    }

    private void Update()
    {
        // keep plane at center
        if (fogPlane)
        {
            fogPlane.transform.position = new Vector3(levelMidPoint.position.x, levelMidPoint.position.y, -5f);
        }

        // per-frame smoothing
        UpdateFogPlaneTextureBuffer();

        // refresh gate
        refreshTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, FogRefreshRate);
        if (refreshTimer < interval) return;
        refreshTimer -= interval; // cancel minor excess

        // skip expensive field update if every revealer is stationary and set to updateOnlyOnMove
        bool needUpdate = false;
        foreach (var r in fogRevealers)
        {
            if (!r._UpdateOnlyOnMove) { needUpdate = true; break; }
            if (r.MovedSinceLastCheck(this)) { needUpdate = true; break; }
        }
        if (!needUpdate) return; // only smoothing already handled above

        UpdateFogField();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || levelMidPoint == null) return;
        Gizmos.color = Color.cyan;
        Vector3 center = levelMidPoint.position;
        Vector3 size = new Vector3(levelDimensionX * unitScale, levelDimensionY * unitScale, 0.01f);
        Gizmos.DrawWireCube(center, size);
    }
    #endregion

    #region === Validate/Init ===
    private void ValidateProperties()
    {
        if (fogRevealers.Any(r => r._RevealerTransform == null))
            Debug.LogError("FogRevealer has missing Transform.");
        if (unitScale <= 0) Debug.LogError("Unit Scale must be > 0.");
        if (scanSpacingPerUnit <= 0) Debug.LogError("Scan Spacing Per Unit must be > 0.");
        if (levelMidPoint == null) Debug.LogError("Level Mid Point is not assigned.");
        if (fogPlaneMaterial == null) Debug.LogError("Fog Plane Material is not assigned.");
    }

    private void InitializeVariables()
    {
        levelData.levelDimensionX = levelDimensionX;
        levelData.levelDimensionY = levelDimensionY;
        levelData.unitScale = unitScale;
        levelData.scanSpacingPerUnit = scanSpacingPerUnit;
    }

    private void InitializeFog()
    {
        // plane (Quad)
        fogPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fogPlane.name = "[RUNTIME] Fog_Quad_2D";
        fogPlane.transform.position = new Vector3(levelMidPoint.position.x, levelMidPoint.position.y, -5f);
        fogPlane.transform.localScale = new Vector3(levelDimensionX * unitScale, levelDimensionY * unitScale, 1f);
        Destroy(fogPlane.GetComponent<Collider>());

        // textures
        fogPlaneTextureLerpTarget = new Texture2D(levelDimensionX, levelDimensionY, TextureFormat.RGBA32, false);
        fogPlaneTextureLerpTarget.wrapMode = TextureWrapMode.Clamp;
        fogPlaneTextureLerpTarget.filterMode = FilterMode.Bilinear;

        fogPlaneTextureLerpBuffer = new Texture2D(levelDimensionX, levelDimensionY, TextureFormat.RGBA32, false);
        fogPlaneTextureLerpBuffer.wrapMode = TextureWrapMode.Clamp;
        fogPlaneTextureLerpBuffer.filterMode = FilterMode.Bilinear;

        // single material instance
        var mr = fogPlane.GetComponent<MeshRenderer>();
        mr.material = new Material(fogPlaneMaterial);
        mr.material.SetColor(ID_Color, fogColor);

        // GPU-lerp support? (shader has _FogTargetTex/_FogBufferTex/_LerpT)
        useGpuLerp = mr.sharedMaterial.HasProperty(ID_Target) && mr.sharedMaterial.HasProperty(ID_Buffer) && mr.sharedMaterial.HasProperty(ID_LerpT);
        if (useGpuLerp)
        {
            mr.material.SetTexture(ID_Target, fogPlaneTextureLerpTarget);
            mr.material.SetTexture(ID_Buffer, fogPlaneTextureLerpBuffer);
            mr.material.SetFloat(ID_LerpT, 1f); // start fully at buffer
        }
        else
        {
            // CPU fallback uses _MainTex to show buffer
            mr.material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);
        }
    }
    #endregion

    #region === Core Update ===
    private void ForceUpdateFog()
    {
        UpdateFogField();
        // reset lerp initial state
        if (useGpuLerp)
        {
            gpuLerpT = 1f;
            var mr = fogPlane.GetComponent<MeshRenderer>();
            mr.material.SetFloat(ID_LerpT, gpuLerpT);
        }
        else
        {
            Graphics.CopyTexture(fogPlaneTextureLerpTarget, fogPlaneTextureLerpBuffer);
        }
    }

    private void UpdateFog()
    {
        // kept for reference (now split across Update/ForceUpdateFog)
    }

    private void UpdateFogField()
    {
        shadowcaster.ResetTileVisibility();
        EnsureSpotArrays();

        // clear weights
        for (int x = 0; x < levelDimensionX; x++)
            for (int y = 0; y < levelDimensionY; y++)
                spotWeight[x, y] = 0f;

        foreach (var r in fogRevealers)
        {
            if (r._RevealerTransform == null) continue;

            // get center cell + world position/forward
            var lc = r.GetCurrentLevelCoordinates(this);
            var revealerPos = new Vector2(GetWorldX(lc.x), GetWorldY(lc.y)); // *** fixed: no double mid-offset ***
            var fwd = r.GetForward2D();

            // determine sampling radius (in tiles)
            int radiusTiles;
            Bounds localBounds;
            if (r.shapeAsset != null)
            {
                localBounds = r.shapeAsset.GetLocalBounds();
                // worst-case radius from local AABB extents, including scale
                var ext = localBounds.extents;
                var sx = Mathf.Abs(r.shapeScale.x); var sy = Mathf.Abs(r.shapeScale.y);
                float rad = Mathf.Max(ext.x * sx, ext.y * sy);
                radiusTiles = Mathf.Max(1, Mathf.RoundToInt(rad / Mathf.Max(0.0001f, unitScale)));
            }
            else
            {
                float outerR = Mathf.Max(0.001f, r.outerRadius);
                radiusTiles = Mathf.Max(1, Mathf.RoundToInt(outerR / Mathf.Max(0.0001f, unitScale)));
                localBounds = new Bounds(Vector3.zero, new Vector3(outerR * 2f, outerR * 2f));
            }

            // build shadowcaster field for this revealer's vicinity
            shadowcaster.ProcessLevelData(lc, radiusTiles);

            int minX = Mathf.Max(0, lc.x - radiusTiles);
            int maxX = Mathf.Min(levelDimensionX - 1, lc.x + radiusTiles);
            int minY = Mathf.Max(0, lc.y - radiusTiles);
            int maxY = Mathf.Min(levelDimensionY - 1, lc.y + radiusTiles);

            // precompute rotation for local transform
            float ang = r.shapeRotationDeg * Mathf.Deg2Rad;
            float ca = Mathf.Cos(ang), sa = Mathf.Sin(ang);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                {
                    // LOS gate: only revealed tiles
                    if (shadowcaster.fogField[x][y] != Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                        continue;

                    Vector2 p = new Vector2(GetWorldX(x), GetWorldY(y));
                    if (!IsVisible(revealerPos, p)) continue; // door blocking

                    float w = 0f;
                    if (r.shapeAsset != null)
                    {
                        // world -> revealer-local (shape space)
                        Vector2 d = p - revealerPos; // translate
                                                     // rotate by local shape rotation (additive to transform.forward). We keep it simple here.
                        var dl = new Vector2(ca * d.x - sa * d.y, sa * d.x + ca * d.y);
                        // apply local offset & scale
                        if (r.shapeScale.x != 0f && r.shapeScale.y != 0f)
                            dl = new Vector2((dl.x - r.shapeOffset.x) / r.shapeScale.x, (dl.y - r.shapeOffset.y) / r.shapeScale.y);
                        w = Mathf.Clamp01(r.shapeAsset.Evaluate(dl, fwd));
                    }
                    else
                    {
                        // Legacy Fan weight
                        float outerR = Mathf.Max(0.001f, r.outerRadius);
                        float innerR = Mathf.Clamp(r.innerRadius, 0f, outerR);
                        float halfOuterA = Mathf.Max(0f, r.outerAngleDeg * 0.5f);
                        float halfInnerA = Mathf.Clamp(r.innerAngleDeg * 0.5f, 0f, halfOuterA);

                        Vector2 to = p - revealerPos; float d2 = to.sqrMagnitude;
                        if (d2 > outerR * outerR) continue;
                        float aDeg = Vector2.Angle(fwd, to); if (aDeg > halfOuterA) continue;
                        float dist01 = (innerR <= 0f) ? 1f : 1f - Mathf.InverseLerp(innerR, outerR, Mathf.Sqrt(d2));
                        float ang01 = (halfInnerA <= 0f) ? 1f : 1f - Mathf.InverseLerp(halfInnerA, halfOuterA, aDeg);
                        w = Mathf.Clamp01(dist01 * ang01);
                    }

                    if (w > spotWeight[x, y]) spotWeight[x, y] = w;
                }
        }

        UpdateFogPlaneTextureTarget();
    }
    #endregion

    #region === Rendering ===
    private void EnsureSpotArrays()
    {
        if (spotWeight == null || spotWeight.GetLength(0) != levelDimensionX || spotWeight.GetLength(1) != levelDimensionY)
            spotWeight = new float[levelDimensionX, levelDimensionY];
    }

    private void UpdateFogPlaneTextureTarget()
    {
        var mr = fogPlane.GetComponent<MeshRenderer>();
        mr.material.SetColor(ID_Color, fogColor);

        var pixels = new Color[levelDimensionX * levelDimensionY];
        int i = 0;
        float revealedA = keepRevealedTiles ? revealedTileOpacity : 0f;

        for (int y = 0; y < levelDimensionY; y++)
            for (int x = 0; x < levelDimensionX; x++, i++)
            {
                float vis = (spotWeight != null) ? spotWeight[x, y] : 0f; // 0..1
                float a = Mathf.Lerp(fogPlaneAlpha, revealedA, vis);
                pixels[i] = new Color(0f, 0f, 0f, a);
            }

        fogPlaneTextureLerpTarget.SetPixels(pixels);
        fogPlaneTextureLerpTarget.Apply(false, false);

        if (useGpuLerp)
        {
            // Reset GPU lerp to start from 0 -> 1 visually
            gpuLerpT = 0f;
            mr.material.SetTexture(ID_Target, fogPlaneTextureLerpTarget);
            mr.material.SetTexture(ID_Buffer, fogPlaneTextureLerpBuffer);
            mr.material.SetFloat(ID_LerpT, gpuLerpT);
        }
        else
        {
            // CPU path: buffer will lerp towards target in UpdateFogPlaneTextureBuffer()
        }
    }

    private void UpdateFogPlaneTextureBuffer()
    {
        var mr = fogPlane ? fogPlane.GetComponent<MeshRenderer>() : null;
        if (useGpuLerp && mr != null)
        {
            gpuLerpT = Mathf.MoveTowards(gpuLerpT, 1f, fogLerpSpeed * Time.deltaTime);
            mr.material.SetFloat(ID_LerpT, gpuLerpT);
            if (gpuLerpT >= 1f)
            {
                // swap: target becomes new buffer to stop re-blending if no field change
                Graphics.CopyTexture(fogPlaneTextureLerpTarget, fogPlaneTextureLerpBuffer);
                mr.material.SetTexture(ID_Buffer, fogPlaneTextureLerpBuffer);
                mr.material.SetFloat(ID_LerpT, 1f);
            }
            return;
        }

        // CPU fallback: lerp in script
        if (fogPlaneTextureLerpBuffer == null || fogPlaneTextureLerpTarget == null) return;
        var bufferPixels = fogPlaneTextureLerpBuffer.GetPixels();
        var targetPixels = fogPlaneTextureLerpTarget.GetPixels();
        if (bufferPixels.Length != targetPixels.Length) return;

        float t = Mathf.Clamp01(fogLerpSpeed * Time.deltaTime);
        for (int i = 0; i < bufferPixels.Length; i++)
            bufferPixels[i] = Color.Lerp(bufferPixels[i], targetPixels[i], t);

        fogPlaneTextureLerpBuffer.SetPixels(bufferPixels);
        fogPlaneTextureLerpBuffer.Apply(false, false);

        if (mr != null) mr.material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);
    }
    #endregion

    #region === Occlusion (Door) ===
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    private bool IsVisible(Vector2 origin, Vector2 point)
    {
        Vector2 dir = (point - origin);
        float dist = dir.magnitude; if (dist <= 1e-4f) return true;
        dir /= dist;

        var filter = new ContactFilter2D();
        filter.SetLayerMask(doorLayers);
        filter.useTriggers = false;
        int count = Physics2D.Raycast(origin, dir, filter, hits, dist);
        return count == 0;
    }
    #endregion

    #region === Grid <-> World ===
    public float GetWorldX(int x)
    {
        // even/odd width handled with same formula as original X (center aligns via levelMidPoint)
        float half = levelDimensionX / 2f;
        // odd width gets +/-0.5 shift on X in original asset; keep consistent with existing visuals
        float centerBias = (levelData.levelDimensionX % 2 == 0) ? 0f : 0.5f;
        return levelMidPoint.position.x + ((x - half) + centerBias) * unitScale;
    }

    public float GetWorldY(int y)
    {
        // *** FIX: odd height uses +0.5f bias like X ***
        float half = levelDimensionY / 2f;
        float centerBias = (levelData.levelDimensionY % 2 == 0) ? 0f : 0.5f;
        return levelMidPoint.position.y + ((y - half) + centerBias) * unitScale;
    }

    public Vector3 GetWorldVector(Vector2Int grid)
    {
        return new Vector3(GetWorldX(grid.x), GetWorldY(grid.y), -5f);
    }

    public int GetUnitX(float worldX)
    {
        float half = levelDimensionX / 2f;
        float centerBias = (levelData.levelDimensionX % 2 == 0) ? 0f : 0.5f;
        return Mathf.RoundToInt((worldX - levelMidPoint.position.x) / unitScale + half - centerBias);
    }
    public int GetUnitY(float worldY)
    {
        float half = levelDimensionY / 2f;
        float centerBias = (levelData.levelDimensionY % 2 == 0) ? 0f : 0.5f;
        return Mathf.RoundToInt((worldY - levelMidPoint.position.y) / unitScale + half - centerBias);
    }

    public Vector2Int WorldToLevel(Vector3 world)
    {
        return new Vector2Int(GetUnitX(world.x), GetUnitY(world.y));
    }

    public bool CheckLevelGridRange(Vector2Int grid)
    {
        bool ok = (grid.x >= 0 && grid.x < levelDimensionX && grid.y >= 0 && grid.y < levelDimensionY);
        if (!ok && LogOutOfRange) Debug.LogWarning($"Grid out of range: {grid}");
        return ok;
    }
    #endregion

    #region === Scan/Save/Load ===
    private void ScanLevel()
    {
        Debug.Log("Scanning level to build obstacle grid...");

        levelData.levelDimensionX = levelDimensionX;
        levelData.levelDimensionY = levelDimensionY;
        levelData.unitScale = unitScale;
        levelData.scanSpacingPerUnit = scanSpacingPerUnit;

        levelData.levelRow.Clear();
        for (int x = 0; x < levelDimensionX; x++)
        {
            var col = new LevelColumn(Enumerable.Repeat(LevelColumn.ETileState.Empty, levelDimensionY));
            levelData.AddColumn(col);
        }

        // sampling world with boxes per tile
        Vector2 size = new Vector2(unitScale / scanSpacingPerUnit, unitScale / scanSpacingPerUnit);
        Vector2 origin0 = new Vector2(GetWorldX(0), GetWorldY(0));

        var cf = new ContactFilter2D();
        cf.SetLayerMask(obstacleLayers);
        cf.useTriggers = !ignoreTriggers;
        var rr = new Collider2D[8];

        for (int x = 0; x < levelDimensionX; x++)
        {
            for (int y = 0; y < levelDimensionY; y++)
            {
                Vector2 center = new Vector2(GetWorldX(x), GetWorldY(y));
                int hits = Physics2D.OverlapBox(center, size, 0f, cf, rr);
                levelData[x][y] = (hits > 0) ? LevelColumn.ETileState.Obstacle : LevelColumn.ETileState.Empty;
            }
        }

        FillEnclosedSpaces();
        if (saveDataOnScan) SaveScanAsLevelData();
        Debug.Log($"Scan complete: {levelDimensionX} x {levelDimensionY}");
    }

    private void FillEnclosedSpaces()
    {
        int X = levelDimensionX, Y = levelDimensionY; // flood from border empties, mark reachable
        bool[,] vis = new bool[X, Y];
        Queue<Vector2Int> q = new Queue<Vector2Int>();

        void TryEnq(int x, int y)
        {
            if (x < 0 || x >= X || y < 0 || y >= Y) return;
            if (vis[x, y]) return;
            if (levelData[x][y] == LevelColumn.ETileState.Obstacle) return;
            vis[x, y] = true; q.Enqueue(new Vector2Int(x, y));
        }

        for (int x = 0; x < X; x++) { TryEnq(x, 0); TryEnq(x, Y - 1); }
        for (int y = 0; y < Y; y++) { TryEnq(0, y); TryEnq(X - 1, y); }

        var dirs = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        while (q.Count > 0)
        {
            var p = q.Dequeue();
            foreach (var d in dirs) TryEnq(p.x + d.x, p.y + d.y);
        }

        // any empty not visited is enclosed; convert to obstacle
        for (int x = 0; x < X; x++)
            for (int y = 0; y < Y; y++)
                if (levelData[x][y] == LevelColumn.ETileState.Empty && !vis[x, y])
                    levelData[x][y] = LevelColumn.ETileState.Obstacle;
    }

    private void SaveScanAsLevelData()
    {
        try
        {
            string dir = Path.Combine(Application.persistentDataPath, "FogLevel");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(Path.Combine(dir, $"{levelNameToSave}.json"), json);
            Debug.Log($"Level saved to {dir}/{levelNameToSave}.json");
        }
        catch (Exception e) { Debug.LogWarning($"Save failed: {e.Message}"); }
    }

    private void LoadLevelData()
    {
        try
        {
            levelData = JsonUtility.FromJson<LevelData>(LevelDataToLoad.text);
            levelDimensionX = levelData.levelDimensionX;
            levelDimensionY = levelData.levelDimensionY;
            unitScale = levelData.unitScale;
            scanSpacingPerUnit = levelData.scanSpacingPerUnit;
            Debug.Log("Level data loaded");
        }
        catch (Exception e) { Debug.LogWarning($"Load failed: {e.Message}"); }
    }
    #endregion


    // === Backward-Compat Shims for Examples ===
    #region BackCompat_ExampleAPIs

    // 예제 스크립트가 쓰는 공개 프로퍼티들
    public Transform _LevelMidPoint => levelMidPoint;
    public List<FogRevealer> _FogRevealers => Revealers;
    public float _UnitScale => unitScale;

    // 예제 스크립트가 쓰는 유틸들
    public bool CheckWorldGridRange(Vector3 worldCoordinates)
    {
        return CheckLevelGridRange(WorldToLevel(worldCoordinates));
    }

    // 구버전 바이너리 가시성(LOS) 쿼리 – 예제에서 fallback으로 사용
    public bool CheckVisibility(Vector3 worldCoordinates, int additionalRadius)
    {
        var lc = WorldToLevel(worldCoordinates);
        if (!CheckLevelGridRange(lc)) return false;

        if (additionalRadius <= 0)
            return shadowcaster.fogField[lc.x][lc.y] ==
                   Shadowcaster.LevelColumn.ETileVisibility.Revealed;

        for (int dx = -additionalRadius; dx <= additionalRadius; dx++)
            for (int dy = -additionalRadius; dy <= additionalRadius; dy++)
            {
                var p = new Vector2Int(lc.x + dx, lc.y + dy);
                if (!CheckLevelGridRange(p)) continue;
                if (shadowcaster.fogField[p.x][p.y] ==
                    Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                    return true;
            }
        return false;
    }

    // 스팟(연속 가시성) 쿼리 – 예제가 reflection으로 찾는 시그니처 그대로
    public bool CheckVisibilitySpot(Vector3 world, int additionalRadius = 0, float threshold = 0.01f)
    {
        if (spotWeight == null) return false;

        var lc = WorldToLevel(world);
        if (!CheckLevelGridRange(lc)) return false;

        if (additionalRadius <= 0)
            return spotWeight[lc.x, lc.y] > threshold;

        for (int dx = -additionalRadius; dx <= additionalRadius; dx++)
            for (int dy = -additionalRadius; dy <= additionalRadius; dy++)
            {
                var p = new Vector2Int(lc.x + dx, lc.y + dy);
                if (!CheckLevelGridRange(p)) continue;
                if (spotWeight[p.x, p.y] > threshold) return true;
            }
        return false;
    }

    // 예제가 호출하는 리빌러 관리 API
    public int AddFogRevealer(FogRevealer r)
    {
        fogRevealers.Add(r);
        return fogRevealers.Count - 1;
    }
    public void RemoveFogRevealer(int index)
    {
        if (index >= 0 && index < fogRevealers.Count) fogRevealers.RemoveAt(index);
        else Debug.LogWarning($"RemoveFogRevealer: index {index} out of range");
    }
    public void ReplaceFogRevealerList(List<FogRevealer> list)
    {
        fogRevealers = list ?? fogRevealers;
    }

    #endregion // BackCompat_ExampleAPIs

}
