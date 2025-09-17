/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csHomebrewFogWar.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using System;                       // Convert
using System.IO;                    // Directory
using System.Linq;                  // Enumerable
using System.Collections.Generic;   // List
using UnityEngine;                  // Monobehaviour
using UnityEditor;                  // Handles



namespace FischlWorks_FogWar
{



    /// The non-static high-level monobehaviour interface of the AOS Fog of War module.

    /// This class holds serialized data for various configuration properties,\n
    /// and is resposible for scanning / saving / loading the LevelData object.\n
    /// The class handles the update frequency of the fog, plus some shader businesses.\n
    /// Various public interfaces related to FogRevealer's FOV are also available.
    public class csFogWar : MonoBehaviour
    {
        private float[,] spotWeight;

        private void EnsureSpotArrays()
        {
            if (spotWeight == null ||
                spotWeight.GetLength(0) != levelDimensionX ||
                spotWeight.GetLength(1) != levelDimensionY)
            {
                spotWeight = new float[levelDimensionX, levelDimensionY];
            }
        }
        /// A class for storing the base level data.
        /// 
        /// This class is later serialized into Json format.\n
        /// Empty spaces are stored as 0, while the obstacles are stored as 1.\n
        /// If a level is loaded instead of being scanned, 
        /// the level dimension properties of csFogWar will be replaced by the level data.
        [System.Serializable]
        public class LevelData
        {
            public void AddColumn(LevelColumn levelColumn)
            {
                levelRow.Add(levelColumn);
            }

            // Indexer definition
            public LevelColumn this[int index]
            {
                get
                {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        return levelRow[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return null;
                    }
                }
                set
                {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        levelRow[index] = value;
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return;
                    }
                }
            }

            // Adding private getter / setters are not allowed for serialization
            public int levelDimensionX = 0;
            public int levelDimensionY = 0;

            public float unitScale = 0;

            public float scanSpacingPerUnit = 0;

            [SerializeField]
            private List<LevelColumn> levelRow = new List<LevelColumn>();
        }

        public bool CheckVisibilitySpot(Vector3 world, int additionalRadius = 0, float threshold = 0.01f)
        {
            // spotWeight가 아직 없다면 그대로 가림 처리
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


        [System.Serializable]
        public class LevelColumn
        {
            public LevelColumn(IEnumerable<ETileState> stateTiles)
            {
                levelColumn = new List<ETileState>(stateTiles);
            }

            // If I create a separate Tile class, it will impact the size of the save file (but enums will be saved as int)
            public enum ETileState
            {
                Empty,
                Obstacle
            }

            // Indexer definition
            public ETileState this[int index]
            {
                get
                {
                    if (index >= 0 && index < levelColumn.Count)
                    {
                        return levelColumn[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in y axis is out of range");

                        return ETileState.Empty;
                    }
                }
                set
                {
                    if (index >= 0 && index < levelColumn.Count)
                    {
                        levelColumn[index] = value;
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in y axis is out of range");

                        return;
                    }
                }
            }

            [SerializeField]
            private List<ETileState> levelColumn = new List<ETileState>();
        }



        [System.Serializable]
        public class FogRevealer
        {
            public FogRevealer(Transform revealerTransform, bool updateOnlyOnMove)
            { this.revealerTransform = revealerTransform; this.updateOnlyOnMove = updateOnlyOnMove; }

            public Vector2Int GetCurrentLevelCoordinates(csFogWar fogWar)
            {
                currentLevelCoordinates = new Vector2Int(
                    fogWar.GetUnitX(revealerTransform.position.x),
                    fogWar.GetUnitY(revealerTransform.position.y));
                return currentLevelCoordinates;
            }

            [SerializeField] private Transform revealerTransform = null;
            public Transform _RevealerTransform => revealerTransform;

            [Header("Spot (Fan) Settings")]
            [Tooltip("월드 단위 거리(바깥 반경)")]
            [SerializeField] public float outerRadius = 7f;
            [Tooltip("월드 단위 거리(안쪽 반경, 0이면 하드 컷)")]
            [SerializeField] public float innerRadius = 3f;
            [Tooltip("바깥 각도(도)")]
            [SerializeField] public float outerAngleDeg = 90f;
            [Tooltip("안쪽 각도(도, 0이면 하드 컷)")]
            [SerializeField] public float innerAngleDeg = 60f;
            [Tooltip("전방 축: true=transform.right, false=transform.up")]
            [SerializeField] public bool forwardIsRight = true;

            [SerializeField] private bool updateOnlyOnMove = true;
            public bool _UpdateOnlyOnMove => updateOnlyOnMove;

            public Vector2 GetForward2D()
            {
                var v = forwardIsRight ? (Vector2)revealerTransform.right : (Vector2)revealerTransform.up;
                if (v.sqrMagnitude < 1e-5f) v = Vector2.right;
                return v.normalized;
            }

            private Vector2Int currentLevelCoordinates = new Vector2Int();
            public Vector2Int _CurrentLevelCoordinates { get { lastSeenAt = currentLevelCoordinates; return currentLevelCoordinates; } }

            [Header("Debug")]
            [SerializeField] private Vector2Int lastSeenAt = new Vector2Int(Int32.MaxValue, Int32.MaxValue);
            public Vector2Int _LastSeenAt => lastSeenAt;
        }



        [BigHeader("Basic Properties")]
        [SerializeField]
        private List<FogRevealer> fogRevealers = null;
        public List<FogRevealer> _FogRevealers => fogRevealers;
        [SerializeField]
        private Transform levelMidPoint = null;
        public Transform _LevelMidPoint => levelMidPoint;
        [SerializeField]
        [Range(1, 30)]
        private float FogRefreshRate = 10;

        [BigHeader("Fog Properties")]

        // 2D Top-Down render settings
        [SerializeField] private float fogPlaneZ = -0.1f;         // 카메라 앞 깊이
        [SerializeField] private string sortingLayerName = "Default";
        [SerializeField] private int sortingOrder = 100;

        [SerializeField]
        [Range(0, 100)]
        private float fogPlaneHeight = 1;
        [SerializeField]
        private Material fogPlaneMaterial = null;
        [SerializeField]
        private Color fogColor = new Color32(5, 15, 25, 255);
        [SerializeField]
        [Range(0, 1)]
        private float fogPlaneAlpha = 1;
        [SerializeField]
        [Range(1, 5)]
        private float fogLerpSpeed = 2.5f;
        public bool keepRevealedTiles = false;
        [ShowIf("keepRevealedTiles")]
        [Range(0, 1)]
        public float revealedTileOpacity = 0.5f;
        [Header("Debug")]
        [SerializeField]
        private Texture2D fogPlaneTextureLerpTarget = null;
        [SerializeField]
        private Texture2D fogPlaneTextureLerpBuffer = null;

        [BigHeader("Level Data")]
        [SerializeField]
        private TextAsset LevelDataToLoad = null;
        [SerializeField]
        private bool saveDataOnScan = true;
        [ShowIf("saveDataOnScan")]
        [SerializeField]
        private string levelNameToSave = "Default";

        [BigHeader("Scan Properties")]
        [SerializeField]
        [Range(1, 128)]
        [Tooltip("If you need more than 128 units, consider using raycasting-based fog modules instead.")]
        private int levelDimensionX = 11;
        [SerializeField]
        [Range(1, 128)]
        [Tooltip("If you need more than 128 units, consider using raycasting-based fog modules instead.")]
        private int levelDimensionY = 11;
        [SerializeField]
        private float unitScale = 1;
        public float _UnitScale => unitScale;
        [SerializeField]
        private float scanSpacingPerUnit = 0.25f;
        [SerializeField]
        private float rayStartHeight = 5;
        [SerializeField]
        private float rayMaxDistance = 10;
        [SerializeField]
        private LayerMask obstacleLayers = new LayerMask();
        [SerializeField]
        private bool ignoreTriggers = true;

        [BigHeader("Debug Options")]
        [SerializeField]
        private bool drawGizmos = false;
        [SerializeField]
        private bool LogOutOfRange = false;

        // External shadowcaster module
        public Shadowcaster shadowcaster { get; private set; } = new Shadowcaster();

        public LevelData levelData { get; private set; } = new LevelData();

        // The primitive plane which will act as a mesh for rendering the fog with
        private GameObject fogPlane = null;

        private float FogRefreshRateTimer = 0;

        private const string levelScanDataPath = "/LevelData";



        // --- --- ---



        private void Start()
        {
            CheckProperties();

            InitializeVariables();

            if (LevelDataToLoad == null)
            {
                ScanLevel();

                if (saveDataOnScan == true)
                {
                    // Preprocessor definitions are used because the save function code will be stripped out on build
#if UNITY_EDITOR
                    SaveScanAsLevelData();
#endif
                }
            }
            else
            {
                LoadLevelData();
            }

            InitializeFog();

            // This part passes the needed references to the shadowcaster
            shadowcaster.Initialize(this);

            // This is needed because we do not update the fog when there's no unit-scale movement of each fogRevealer
            ForceUpdateFog();
        }



        private void Update()
        {
            UpdateFog();
        }



        // --- --- ---



        private void CheckProperties()
        {
            foreach (FogRevealer fogRevealer in fogRevealers)
            {
                if (fogRevealer._RevealerTransform == null)
                {
                    Debug.LogErrorFormat("Please assign a Transform component to each Fog Revealer!");
                }
            }

            if (unitScale <= 0)
            {
                Debug.LogErrorFormat("Unit Scale must be bigger than 0!");
            }

            if (scanSpacingPerUnit <= 0)
            {
                Debug.LogErrorFormat("Scan Spacing Per Unit must be bigger than 0!");
            }

            if (levelMidPoint == null)
            {
                Debug.LogErrorFormat("Please assign the Level Mid Point property!");
            }

            if (fogPlaneMaterial == null)
            {
                Debug.LogErrorFormat("Please assign the \"FogPlane\" material to the Fog Plane Material property!");
            }
        }



        private void InitializeVariables()
        {
            // This is for faster development iteration purposes
            if (obstacleLayers.value == 0)
            {
                obstacleLayers = LayerMask.GetMask("Default");
            }

            // This is also for faster development iteration purposes
            if (levelNameToSave == String.Empty)
            {
                levelNameToSave = "Default";
            }
        }



        private void InitializeFog()
        {
            fogPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fogPlane.name = "[RUNTIME] Fog_Quad_2D";

            // XY 평면 정렬, 카메라 앞 z 오프셋
            fogPlane.transform.position = new Vector3(
                levelMidPoint.position.x,
                levelMidPoint.position.y,
                fogPlaneZ);

            // Quad는 1x1 기준 → 월드 크기 그대로
            fogPlane.transform.localScale = new Vector3(
                levelDimensionX * unitScale,
                levelDimensionY * unitScale,
                1f);

            var mr = fogPlane.GetComponent<MeshRenderer>();
            mr.material = new Material(fogPlaneMaterial);
            mr.material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);

            //fogPlane.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            // 2D 정렬(스프라이트 위로)
            mr.sortingLayerName = sortingLayerName;
            mr.sortingOrder = sortingOrder;

            // MeshCollider는 꺼두기
            var mc = fogPlane.GetComponent<MeshCollider>();
            if (mc) mc.enabled = false;


            fogPlaneTextureLerpTarget = new Texture2D(levelDimensionX, levelDimensionY);
            fogPlaneTextureLerpBuffer = new Texture2D(levelDimensionX, levelDimensionY);

            fogPlaneTextureLerpBuffer.wrapMode = TextureWrapMode.Clamp;

            fogPlaneTextureLerpBuffer.filterMode = FilterMode.Bilinear;

            fogPlane.GetComponent<MeshRenderer>().material = new Material(fogPlaneMaterial);

            fogPlane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);

            fogPlane.GetComponent<MeshCollider>().enabled = false;
        }



        private void ForceUpdateFog()
        {
            UpdateFogField();

            Graphics.CopyTexture(fogPlaneTextureLerpTarget, fogPlaneTextureLerpBuffer);
        }



        private void UpdateFog()
        {
            fogPlane.transform.position = new Vector3(
                levelMidPoint.position.x,
                levelMidPoint.position.y,
                fogPlaneZ);

            FogRefreshRateTimer += Time.deltaTime;

            if (FogRefreshRateTimer < 1 / FogRefreshRate)
            {
                UpdateFogPlaneTextureBuffer();

                return;
            }
            else
            {
                // This is to cancel out minor excess values
                FogRefreshRateTimer -= 1 / FogRefreshRate;
            }

            foreach (FogRevealer fogRevealer in fogRevealers)
            {
                if (fogRevealer._UpdateOnlyOnMove == false)
                {
                    break;
                }

                Vector2Int currentLevelCoordinates = fogRevealer.GetCurrentLevelCoordinates(this);

                if (currentLevelCoordinates != fogRevealer._LastSeenAt)
                {
                    break;
                }

                if (fogRevealer == fogRevealers.Last())
                {
                    return;
                }
            }

            UpdateFogField();

            UpdateFogPlaneTextureBuffer();
        }



        private void UpdateFogField()
        {
            shadowcaster.ResetTileVisibility();
            EnsureSpotArrays();

            // 가중치 초기화
            for (int x = 0; x < levelDimensionX; x++)
                for (int y = 0; y < levelDimensionY; y++)
                    spotWeight[x, y] = 0f;

            foreach (var r in fogRevealers)
            {
                // 중심 타일/월드 좌표/전방
                var lc = r.GetCurrentLevelCoordinates(this);
                Vector2 revealerPos = new Vector2(GetWorldX(lc.x + (levelDimensionX / 2)),
                                                  GetWorldY(lc.y + (levelDimensionY / 2)));
                Vector2 fwd = r.GetForward2D();

                // 반경/각도
                float outerR = Mathf.Max(0.001f, r.outerRadius);
                float innerR = Mathf.Clamp(r.innerRadius, 0f, outerR);
                float halfOuterA = Mathf.Max(0f, r.outerAngleDeg * 0.5f);
                float halfInnerA = Mathf.Clamp(r.innerAngleDeg * 0.5f, 0f, halfOuterA);

                int radiusTiles = Mathf.Max(1, Mathf.RoundToInt(outerR / unitScale));

                // LOS(가림) 계산은 반경 타일로
                shadowcaster.ProcessLevelData(lc, radiusTiles);

                // 부채꼴 가중치(거리/각도 소프트 에지)
                int minX = Mathf.Max(0, lc.x - radiusTiles);
                int maxX = Mathf.Min(levelDimensionX - 1, lc.x + radiusTiles);
                int minY = Mathf.Max(0, lc.y - radiusTiles);
                int maxY = Mathf.Min(levelDimensionY - 1, lc.y + radiusTiles);

                float outerR2 = outerR * outerR;

                for (int x = minX; x <= maxX; x++)
                    for (int y = minY; y <= maxY; y++)
                    {
                        // LOS가 안 뚫린 타일이면 어차피 0이 될 것이니, 가벼운 필터
                        if (shadowcaster.fogField[x][y] != Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                            continue;

                        Vector2 p = new Vector2(GetWorldX(x), GetWorldY(y));
                        Vector2 to = p - revealerPos;
                        float d2 = to.sqrMagnitude;
                        if (d2 > outerR2) continue;

                        float ang = Vector2.Angle(fwd, to);
                        if (ang > halfOuterA) continue;

                        float dist01 = (innerR <= 0f) ? 1f : 1f - Mathf.InverseLerp(innerR, outerR, Mathf.Sqrt(d2));
                        float ang01 = (halfInnerA <= 0f) ? 1f : 1f - Mathf.InverseLerp(halfInnerA, halfOuterA, ang);

                        float w = Mathf.Clamp01(dist01 * ang01);

                        if (w > spotWeight[x, y]) spotWeight[x, y] = w; // 여러 리빌러면 최대값
                    }
            }

            UpdateFogPlaneTextureTarget();
        }




        // Doing shader business on the script, if we pull this out as a shader pass, same operations must be repeated
        private void UpdateFogPlaneTextureBuffer()
        {
            Color[] bufferPixels = fogPlaneTextureLerpBuffer.GetPixels();
            Color[] targetPixels = fogPlaneTextureLerpTarget.GetPixels();

            if (bufferPixels.Length != targetPixels.Length)
            {
                Debug.LogErrorFormat("Fog plane texture buffer and target have different pixel counts");
                return;
            }

            for (int i = 0; i < bufferPixels.Length; i++)
            {
                bufferPixels[i] = Color.Lerp(bufferPixels[i], targetPixels[i], fogLerpSpeed * Time.deltaTime);
            }

            fogPlaneTextureLerpBuffer.SetPixels(bufferPixels);

            fogPlaneTextureLerpBuffer.Apply();
        }



        private void UpdateFogPlaneTextureTarget()
        {
            var mr = fogPlane.GetComponent<MeshRenderer>();
            mr.material.SetColor("_Color", fogColor);

            var pixels = new Color[levelDimensionX * levelDimensionY];
            int i = 0;
            float revealedA = keepRevealedTiles ? revealedTileOpacity : 0f;

            for (int y = 0; y < levelDimensionY; y++)
                for (int x = 0; x < levelDimensionX; x++, i++)
                {
                    // spotWeight는 UpdateFogField에서 LOS 통과한 타일만 채움
                    float vis = (spotWeight != null) ? spotWeight[x, y] : 0f;
                    // vis=1 → revealedA(투명/회색), vis=0 → fogPlaneAlpha(어두움)
                    float a = Mathf.Lerp(fogPlaneAlpha, revealedA, vis);
                    pixels[i] = new Color(1f, 1f, 1f, a);
                }

            if (fogPlaneTextureLerpTarget == null ||
                fogPlaneTextureLerpTarget.width != levelDimensionX ||
                fogPlaneTextureLerpTarget.height != levelDimensionY)
            {
                fogPlaneTextureLerpTarget = new Texture2D(levelDimensionX, levelDimensionY, TextureFormat.RGBA32, false);
                fogPlaneTextureLerpTarget.wrapMode = TextureWrapMode.Clamp;
                fogPlaneTextureLerpTarget.filterMode = FilterMode.Point;
            }

            fogPlaneTextureLerpTarget.SetPixels(pixels);
            fogPlaneTextureLerpTarget.Apply();
        }




        private void ScanLevel()
        {
            Debug.LogFormat("There is no level data file assigned, scanning level...");

            // These operations have no real computational meaning, but it will bring consistency to the data
            levelData.levelDimensionX = levelDimensionX;
            levelData.levelDimensionY = levelDimensionY;
            levelData.unitScale = unitScale;
            levelData.scanSpacingPerUnit = scanSpacingPerUnit;

            for (int xIterator = 0; xIterator < levelDimensionX; xIterator++)
            {
                // Adding a new list for column (y axis) for each unit in row (x axis)
                levelData.AddColumn(new LevelColumn(Enumerable.Repeat(LevelColumn.ETileState.Empty, levelDimensionY)));

                for (int yIterator = 0; yIterator < levelDimensionY; yIterator++)
                {
                    Vector2 center = new Vector2(GetWorldX(xIterator), GetWorldY(yIterator));
                    Vector2 size = new Vector2(unitScale - scanSpacingPerUnit, unitScale - scanSpacingPerUnit);
                    var hits = Physics2D.OverlapBoxAll(center, size, 0f, obstacleLayers);

                    bool isObstacleHit = false;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (!hits[i]) continue;
                        if (ignoreTriggers && hits[i].isTrigger) continue;
                        isObstacleHit = true; break;
                    }
                    if (isObstacleHit)
                    {
                        levelData[xIterator][yIterator] = LevelColumn.ETileState.Obstacle;
                    }

                }
            }

            Debug.LogFormat("Successfully scanned level with a scale of {0} x {1}", levelDimensionX, levelDimensionY);
        }



        // We intend to use Application.dataPath only for accessing project files directory (only in unity editor)
#if UNITY_EDITOR
        private void SaveScanAsLevelData()
        {
            string fullPath = Application.dataPath + levelScanDataPath + "/" + levelNameToSave + ".json";

            if (Directory.Exists(Application.dataPath + levelScanDataPath) == false)
            {
                Directory.CreateDirectory(Application.dataPath + levelScanDataPath);

                Debug.LogFormat("level scan data folder at \"{0}\" is missing, creating...", levelScanDataPath);
            }

            if (File.Exists(fullPath) == true)
            {
                Debug.LogFormat("level scan data already exists, overwriting...");
            }

            string levelJson = JsonUtility.ToJson(levelData);

            File.WriteAllText(fullPath, levelJson);

            Debug.LogFormat("Successfully saved level scan data at \"{0}\"", fullPath);
        }
#endif



        private void LoadLevelData()
        {
            Debug.LogFormat("Level scan data with a name of \"{0}\" is assigned, loading...", LevelDataToLoad.name);

            // Exception check is indirectly performed through branching on the upper part of the code
            string levelJson = LevelDataToLoad.ToString();

            levelData = JsonUtility.FromJson<LevelData>(levelJson);

            levelDimensionX = levelData.levelDimensionX;
            levelDimensionY = levelData.levelDimensionY;
            unitScale = levelData.unitScale;
            scanSpacingPerUnit = levelData.scanSpacingPerUnit;

            Debug.LogFormat("Successfully loaded level scan data with the name of \"{0}\"", LevelDataToLoad.name);
        }



        /// Adds a new FogRevealer instance to the list and returns its index
        public int AddFogRevealer(FogRevealer fogRevealer)
        {
            fogRevealers.Add(fogRevealer);

            return fogRevealers.Count - 1;
        }



        /// Removes a FogRevealer instance from the list with index
        public void RemoveFogRevealer(int revealerIndex)
        {
            if (fogRevealers.Count > revealerIndex && revealerIndex > -1)
            {
                fogRevealers.RemoveAt(revealerIndex);
            }
            else
            {
                Debug.LogFormat("Given index of {0} exceeds the revealers' container range", revealerIndex);
            }
        }



        /// Replaces the FogRevealer list with the given one
        public void ReplaceFogRevealerList(List<FogRevealer> fogRevealers)
        {
            this.fogRevealers = fogRevealers;
        }



        /// Checks if the given level coordinates are within level dimension range.
        public bool CheckLevelGridRange(Vector2Int levelCoordinates)
        {
            bool result =
                levelCoordinates.x >= 0 &&
                levelCoordinates.x < levelData.levelDimensionX &&
                levelCoordinates.y >= 0 &&
                levelCoordinates.y < levelData.levelDimensionY;

            if (result == false && LogOutOfRange == true)
            {
                Debug.LogFormat("Level coordinates \"{0}\" is out of grid range", levelCoordinates);
            }

            return result;
        }



        /// Checks if the given world coordinates are within level dimension range.
        public bool CheckWorldGridRange(Vector3 worldCoordinates)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            return CheckLevelGridRange(levelCoordinates);
        }



        /// Checks if the given pair of world coordinates and additionalRadius is visible by FogRevealers.
        public bool CheckVisibility(Vector3 worldCoordinates, int additionalRadius)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            if (additionalRadius == 0)
            {
                return shadowcaster.fogField[levelCoordinates.x][levelCoordinates.y] ==
                    Shadowcaster.LevelColumn.ETileVisibility.Revealed;
            }

            int scanResult = 0;

            for (int xIterator = -1; xIterator < additionalRadius + 1; xIterator++)
            {
                for (int yIterator = -1; yIterator < additionalRadius + 1; yIterator++)
                {
                    if (CheckLevelGridRange(new Vector2Int(
                        levelCoordinates.x + xIterator,
                        levelCoordinates.y + yIterator)) == false)
                    {
                        scanResult = 0;

                        break;
                    }

                    scanResult += Convert.ToInt32(
                        shadowcaster.fogField[levelCoordinates.x + xIterator][levelCoordinates.y + yIterator] ==
                        Shadowcaster.LevelColumn.ETileVisibility.Revealed);
                }
            }

            if (scanResult > 0)
            {
                return true;
            }

            return false;
        }



        /// Converts unit (divided by unitScale, then rounded) world coordinates to level coordinates.
        public Vector2Int WorldToLevel(Vector3 worldCoordinates)
        {
            Vector2Int unitWorldCoordinates = GetUnitVector(worldCoordinates);

            return new Vector2Int(
                unitWorldCoordinates.x + (levelDimensionX / 2),
                unitWorldCoordinates.y + (levelDimensionY / 2));
        }



        /// Converts level coordinates into world coordinates.
        public Vector3 GetWorldVector(Vector2Int worldCoordinates)
        {
            return new Vector3(
                GetWorldX(worldCoordinates.x + (levelDimensionX / 2)),
                GetWorldY(worldCoordinates.y + (levelDimensionY / 2)),
                fogPlaneZ);

        }



        /// Converts "pure" world coordinates into unit world coordinates.
        public Vector2Int GetUnitVector(Vector3 worldCoordinates)
        {
            return new Vector2Int(GetUnitX(worldCoordinates.x), GetUnitY(worldCoordinates.y));
        }



        /// Converts level coordinate to corresponding unit world coordinates.
        public float GetWorldX(int xValue)
        {
            if (levelData.levelDimensionX % 2 == 0)
            {
                return (levelMidPoint.position.x - ((levelDimensionX / 2.0f) - xValue) * unitScale);
            }

            return (levelMidPoint.position.x - ((levelDimensionX / 2.0f) - (xValue + 0.5f)) * unitScale);
        }



        /// Converts world coordinate to unit world coordinates.
        public int GetUnitX(float xValue)
        {
            return Mathf.RoundToInt((xValue - levelMidPoint.position.x) / unitScale);
        }



        /// Converts level coordinate to corresponding unit world coordinates.
        public float GetWorldY(int yValue)
        {
            if (levelData.levelDimensionY % 2 == 0)
            {
                return (levelMidPoint.position.y - ((levelDimensionY / 2.0f) - yValue) * unitScale);
            }

            return (levelMidPoint.position.y - ((levelDimensionY / 2.0f) - yValue) * unitScale);
        }



        /// Converts world coordinate to unit world coordinates.
        public int GetUnitY(float yValue)
        {
            return Mathf.RoundToInt((yValue - levelMidPoint.position.y) / unitScale);
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            if (drawGizmos == false)
            {
                return;
            }

            Handles.color = Color.yellow;

            for (int xIterator = 0; xIterator < levelDimensionX; xIterator++)
            {
                for (int yIterator = 0; yIterator < levelDimensionY; yIterator++)
                {
                    if (levelData[xIterator][yIterator] == LevelColumn.ETileState.Obstacle)
                    {
                        if (shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                        {
                            Handles.color = Color.green;
                        }
                        else
                        {
                            Handles.color = Color.red;
                        }

                        Handles.DrawWireCube(
new Vector3(GetWorldX(xIterator), GetWorldY(yIterator), fogPlaneZ),
                            new Vector3(
                                unitScale - scanSpacingPerUnit,
                                unitScale,
                                unitScale - scanSpacingPerUnit));
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;

                        Gizmos.DrawSphere(
new Vector3(GetWorldX(xIterator), GetWorldY(yIterator), fogPlaneZ),
                            unitScale / 5.0f);
                    }

                    if (shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                    {
                        Gizmos.color = Color.green;

                        Gizmos.DrawSphere(
new Vector3(GetWorldX(xIterator), GetWorldY(yIterator), fogPlaneZ),
                            unitScale / 3.0f);
                    }
                }
            }
        }
#endif
    }



    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string _BaseCondition
        {
            get { return mBaseCondition; }
        }

        private string mBaseCondition = String.Empty;

        public ShowIfAttribute(string baseCondition)
        {
            mBaseCondition = baseCondition;
        }
    }



    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BigHeaderAttribute : PropertyAttribute
    {
        public string _Text
        {
            get { return mText; }
        }

        private string mText = String.Empty;

        public BigHeaderAttribute(string text)
        {
            mText = text;
        }
    }



}