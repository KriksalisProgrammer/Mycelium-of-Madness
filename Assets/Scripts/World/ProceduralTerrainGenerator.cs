using UnityEngine;

[ExecuteInEditMode]
public class ProceduralTerrainGenerator : MonoBehaviour
{
    public static ProceduralTerrainGenerator Instance { get; private set; }

    [Header("Terrain References")]
    public Terrain terrain;

    [Header("Base Settings")]
    public int resolution = 513;
    public float terrainSize = 500f;
    public float maxHeight = 55f;

    [Header("Small Hills Noise")]
    [Range(20f, 150f)] public float noiseScale = 68f;
    [Range(1, 6)] public int octaves = 4;
    [Range(0.1f, 1f)] public float persistence = 0.52f;
    [Range(1f, 3f)] public float lacunarity = 1.95f;
    public int seed = 42;

    [Header("World Structure")]
    public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0.15f, 1, 1f);
    public bool useFalloffMap = true;
    [Range(1f, 5f)] public float falloffSteepness = 2.3f;

    [Header("Lake")]
    public bool generateLake = true;
    [Range(0.15f, 0.45f)] public float lakeRadius = 0.29f;
    [Range(0.02f, 0.16f)] public float lakeEdgeSoftness = 0.095f;
    [Range(0.18f, 0.45f)] public float lakeDepth = 0.37f;
    [Range(0.08f, 0.28f)] public float baseWaterLevel = 0.17f;
    public Material waterMaterial;

    [Header("River from Lake")]
    public bool generateRiver = true;
    [Range(4f, 14f)] public float riverWidth = 8f;
    [Range(0.12f, 0.38f)] public float riverDepth = 0.26f;
    [Range(0.05f, 0.35f)] public float riverWiggleStrength = 0.18f;
    [Range(8, 25)] public int riverSegments = 18;

    [Header("Post Generation")]
    public bool autoSpawnObjects = true;

    private GameObject lakeObject;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void GenerateTerrain()
    {
        if (terrain == null) { Debug.LogError("[Terrain] Terrain не назначен!"); return; }

        TerrainData td = terrain.terrainData;
        td.heightmapResolution = resolution;
        td.size = new Vector3(terrainSize, maxHeight, terrainSize);
        
        float[,] heights = GenerateHeightMap();

        if (generateLake)
            ApplyLakeToHeights(heights);

        td.SetHeights(0, 0, heights);
        terrain.Flush();
        
        if (generateLake) SpawnLake();

        if (generateRiver) CarveRiver();

        if (Application.isPlaying && autoSpawnObjects && MushroomSpawner.Instance != null)
            MushroomSpawner.Instance.SpawnMushroomsForNewDay();

        Debug.Log($"[Terrain] Готово! Seed={seed} | Озеро + Река");
    }

    private float[,] GenerateHeightMap()
    {
        int res = terrain.terrainData.heightmapResolution;
        float[,] heights = new float[res, res];

        Random.InitState(seed);
        float ox = Random.Range(-10000f, 10000f);
        float oy = Random.Range(-10000f, 10000f);

        for (int x = 0; x < res; x++)
        for (int y = 0; y < res; y++)
        {
            float amp = 1f, freq = 1f, noise = 0f;
            for (int i = 0; i < octaves; i++)
            {
                float px = (x / noiseScale) * freq + ox;
                float py = (y / noiseScale) * freq + oy;
                noise += (Mathf.PerlinNoise(px, py) * 2f - 1f) * amp;
                amp *= persistence;
                freq *= lacunarity;
            }

            float h = heightCurve.Evaluate(Mathf.InverseLerp(-1.6f, 1.6f, noise));

            if (useFalloffMap) h = ApplyFalloff(h, x, y, res);

            heights[x, y] = Mathf.Clamp01(h);
        }
        return heights;
    }

    private float ApplyFalloff(float h, int x, int y, int res)
    {
        float nx = (x / (float)res) * 2f - 1f;
        float ny = (y / (float)res) * 2f - 1f;
        float v = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
        float falloff = Mathf.Pow(v, falloffSteepness) / (Mathf.Pow(v, falloffSteepness) + Mathf.Pow(1f - v, falloffSteepness));
        return Mathf.Lerp(h, 0.13f, falloff * 0.58f);
    }
    
    private void ApplyLakeToHeights(float[,] heights)
    {
        int res = heights.GetLength(0);

        for (int x = 0; x < res; x++)
        for (int y = 0; y < res; y++)
        {
            float cx = (x / (float)res) * 2f - 1f;
            float cy = (y / (float)res) * 2f - 1f;
            float dist = Mathf.Sqrt(cx * cx + cy * cy);

            float mask = 1f - Mathf.SmoothStep(lakeRadius - lakeEdgeSoftness, lakeRadius + lakeEdgeSoftness, dist);

            float depthFactor = Mathf.Pow(1f - Mathf.Clamp01(dist / lakeRadius), 1.6f);
            float lakeBottom = baseWaterLevel - (lakeDepth * depthFactor);

            heights[x, y] = Mathf.Lerp(heights[x, y], Mathf.Max(0.02f, lakeBottom), mask * 0.92f);
        }
    }

    private void SpawnLake()
    {
        if (!generateLake) return;

        if (lakeObject != null)
        {
            if (Application.isPlaying) Destroy(lakeObject);
            else DestroyImmediate(lakeObject);
        }

        float waterY = terrain.transform.position.y + baseWaterLevel * maxHeight;
        float centerX = terrain.transform.position.x + terrainSize * 0.5f;
        float centerZ = terrain.transform.position.z + terrainSize * 0.5f;

        float planeSize = lakeRadius * terrainSize * 2f * 0.98f;

        lakeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        lakeObject.name = "Lake_Water";
        lakeObject.transform.position = new Vector3(centerX, waterY + 0.1f, centerZ);
        lakeObject.transform.localScale = new Vector3(planeSize / 10f, 1f, planeSize / 10f);

        var rend = lakeObject.GetComponent<Renderer>();
        rend.sharedMaterial = waterMaterial != null ? waterMaterial : CreateFallbackWaterMat();

        var col = lakeObject.GetComponent<Collider>();
        if (col != null) DestroyImmediate(col);
    }

    private Material CreateFallbackWaterMat()
    {
        var mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.06f, 0.28f, 0.68f, 0.82f);
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = 3000;
        return mat;
    }
    
    private void CarveRiver()
    {
        if (!generateRiver) return;

        int res = terrain.terrainData.heightmapResolution;
        float[,] heights = terrain.terrainData.GetHeights(0, 0, res, res);

        Vector2 start = new Vector2(res * 0.5f, res * 0.5f);

        int edge = Random.Range(0, 4);
        Vector2 end = edge switch
        {
            0 => new Vector2(res * 0.5f, res * 0.03f),
            1 => new Vector2(res * 0.97f, res * 0.5f),
            2 => new Vector2(res * 0.5f, res * 0.97f),
            _ => new Vector2(res * 0.03f, res * 0.5f)
        };

        Vector2 current = start;
        float step = terrainSize / resolution * 2.8f;

        for (int i = 0; i < riverSegments; i++)
        {
            Vector2 dir = (end - current).normalized;

            float wiggle = (Mathf.PerlinNoise(current.x * 0.08f + seed, current.y * 0.08f + seed) - 0.5f) * riverWiggleStrength;
            dir = Quaternion.Euler(0, 0, wiggle * 40f) * new Vector3(dir.x, dir.y);

            current += dir * step;

            int cx = Mathf.Clamp((int)current.x, 0, res - 1);
            int cy = Mathf.Clamp((int)current.y, 0, res - 1);

            float halfWidth = riverWidth / (terrainSize / res) * 0.5f;

            for (int dx = -(int)halfWidth - 1; dx <= (int)halfWidth + 1; dx++)
            for (int dy = -(int)halfWidth - 1; dy <= (int)halfWidth + 1; dy++)
            {
                int px = Mathf.Clamp(cx + dx, 0, res - 1);
                int py = Mathf.Clamp(cy + dy, 0, res - 1);

                float dist = Mathf.Sqrt(dx * dx + dy * dy) / halfWidth;
                float depthFalloff = Mathf.Exp(-dist * dist * 3.2f);

                heights[px, py] = Mathf.Max(0.02f, heights[px, py] - riverDepth * depthFalloff);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        Debug.Log("[Terrain] Река вырезана");
    }

    [ContextMenu("Generate Terrain (Озеро + Река)")]
    public void DebugGenerate() => GenerateTerrain();

    [ContextMenu("New Random Seed & Generate")]
    public void RandomSeedAndGenerate()
    {
        seed = Random.Range(0, 999999);
        GenerateTerrain();
    }
}