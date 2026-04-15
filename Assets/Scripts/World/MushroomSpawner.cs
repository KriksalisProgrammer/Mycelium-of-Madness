using UnityEngine;
using System.Collections.Generic;

public class MushroomSpawner : MonoBehaviour
{
    public static MushroomSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject mushroomPrefab;           
    public int mushroomsPerDay = 15;           
    public float minDistanceBetweenMushrooms = 2.5f; 

    [Header("Terrain")]
    public Terrain terrain;                     
    public LayerMask groundLayer;              

    [Header("Height & Area")]
    public float minHeight = 0f;                
    public float maxHeight = 100f;
    public Vector2 spawnAreaMin = new Vector2(-200, -200);
    public Vector2 spawnAreaMax = new Vector2(200, 200);

    private List<GameObject> activeMushrooms = new List<GameObject>();
    private Queue<GameObject> mushroomPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (terrain == null)
            terrain = FindObjectOfType<Terrain>();
    }

   
    public void SpawnMushroomsForNewDay()
    {
        ClearCurrentMushrooms();

        int spawned = 0;
        int attempts = 0;
        const int maxAttempts = 500;

        while (spawned < mushroomsPerDay && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPos = GetRandomPositionOnTerrain();
            if (randomPos == Vector3.zero) continue; 

            if (IsTooCloseToOtherMushrooms(randomPos)) continue;

            GameObject mushroom = GetFromPool();
            mushroom.transform.position = randomPos + Vector3.up * 0.1f; 
            mushroom.SetActive(true);

            activeMushrooms.Add(mushroom);
            spawned++;
        }

        Debug.Log($"[MushroomSpawner] Spawned {spawned} mushrooms for new day.");
    }

    private Vector3 GetRandomPositionOnTerrain()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector3 rayOrigin = new Vector3(randomX, 1000f, randomZ);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 2000f, groundLayer))
        {
            if (hit.point.y >= minHeight && hit.point.y <= maxHeight)
                return hit.point;
        }
        }

        return Vector3.zero;
    }

    private bool IsTooCloseToOtherMushrooms(Vector3 position)
    {
        foreach (var mush in activeMushrooms)
        {
            if (Vector3.Distance(position, mush.transform.position) < minDistanceBetweenMushrooms)
                return true;
        }
        return false;
    }

    private GameObject GetFromPool()
    {
        if (mushroomPool.Count > 0)
        {
            return mushroomPool.Dequeue();
        }
        
        GameObject newMush = Instantiate(mushroomPrefab);
        newMush.SetActive(false);
        return newMush;
    }

    public void ReturnToPool(GameObject mushroom)
    {
        mushroom.SetActive(false);
        mushroomPool.Enqueue(mushroom);
        activeMushrooms.Remove(mushroom);
    }

    private void ClearCurrentMushrooms()
    {
        activeMushrooms.Clear();
    }
    
    [ContextMenu("Spawn Now")]
    public void DebugSpawnNow() => SpawnMushroomsForNewDay();
}