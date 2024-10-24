using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProGenManager : MonoBehaviour
{
    #region Singleton
    public static ProGenManager proGenManagerInstance { get; private set; }
    #endregion

    #region Events
    public event Action<GameObject> OnCorridorSpawned;
    public event Action<GameObject> OnCorridorDespawned;
    #endregion

    #region Configuration
    [SerializeField] private ProGenConfiguration config;
    [SerializeField] private GameObject initialCorridorSection;
    #endregion

    #region Private Fields
    private Dictionary<string, Queue<GameObject>> corridorPool;
    private List<GameObject> activeCorridors = new List<GameObject>();
    private float nextSpawnZ = 0f;
    private Queue<bool> obstaclePattern;
    #endregion

    private void Awake()
    {
        InitializeSingleton();
        InitializeObjectPool();
    }

    private void Start()
    {
        TrackInitialCorridor();
        SpawnInitialCorridors();
        GenerateObstaclePattern(10); // Generate initial pattern
    }

    #region Initialization Methods
    private void InitializeSingleton()
    {
        if (proGenManagerInstance == null)
        {
            proGenManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitializeObjectPool()
    {
        corridorPool = new Dictionary<string, Queue<GameObject>>();
        
        // Initialize pools for corridor sections
        foreach (GameObject corridor in config.corridorSections)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < config.maxActiveCorridors; i++)
            {
                GameObject obj = CreatePoolObject(corridor);
                pool.Enqueue(obj);
            }
            corridorPool.Add(corridor.name, pool);
        }

        // Initialize pools for obstacle sections
        foreach (GameObject obstacle in config.obstacleSections)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < config.maxActiveCorridors; i++)
            {
                GameObject obj = CreatePoolObject(obstacle);
                pool.Enqueue(obj);
            }
            corridorPool.Add(obstacle.name, pool);
        }
    }

    private GameObject CreatePoolObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        return obj;
    }
    #endregion

    #region Corridor Management
    private void TrackInitialCorridor()
    {
        if (initialCorridorSection != null)
        {
            activeCorridors.Add(initialCorridorSection);
            nextSpawnZ = initialCorridorSection.transform.position.z + config.spawnDistance;
        }
        else
        {
            Debug.LogError("Initial corridor section is not assigned!");
        }
    }

    private void SpawnInitialCorridors()
    {
        for (int i = 0; i < config.initialCorridorCount; i++)
        {
            SpawnCorridor();
        }
    }

    public void SpawnCorridor()
    {
        bool isObstacle = obstaclePattern.Dequeue();
        GameObject[] sectionArray = isObstacle ? config.obstacleSections : config.corridorSections;
        
        int randomIndex = UnityEngine.Random.Range(0, sectionArray.Length);
        GameObject prefab = sectionArray[randomIndex];
        
        // Get object from pool
        GameObject newSection = GetPooledObject(prefab.name);
        newSection.transform.position = new Vector3(0, 0, nextSpawnZ);
        newSection.SetActive(true);

        activeCorridors.Add(newSection);
        nextSpawnZ += config.spawnDistance;
        
        OnCorridorSpawned?.Invoke(newSection);

        if (activeCorridors.Count > config.maxActiveCorridors)
        {
            _ = UnloadChunkAsync(activeCorridors[0]);
        }

        // Regenerate pattern if needed
        if (obstaclePattern.Count == 0)
        {
            GenerateObstaclePattern(10);
        }
    }

    private GameObject GetPooledObject(string prefabName)
    {
        if (corridorPool.TryGetValue(prefabName, out Queue<GameObject> pool))
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
        }
        
        Debug.LogWarning($"Pool for {prefabName} is empty, creating new object");
        return CreatePoolObject(Array.Find(config.corridorSections, x => x.name == prefabName));
    }
    #endregion

    #region Async Operations
    private async Task UnloadChunkAsync(GameObject corridor)
    {
        await Task.Delay(3000);
        corridor.SetActive(false);
        
        // Return to pool
        string prefabName = corridor.name.Replace("(Clone)", "").Trim();
        corridorPool[prefabName].Enqueue(corridor);
        
        activeCorridors.Remove(corridor);
        OnCorridorDespawned?.Invoke(corridor);
    }
    #endregion

    #region Pattern Generation
    private void GenerateObstaclePattern(int length)
    {
        obstaclePattern = new Queue<bool>();
        float previousValue = 0;
        
        for (int i = 0; i < length; i++)
        {
            // Use Perlin noise for more natural pattern generation
            float noise = Mathf.PerlinNoise(previousValue, 0);
            bool isObstacle = noise < (config.obstacleChanceThreshold / 10f);
            obstaclePattern.Enqueue(isObstacle);
            previousValue += 0.5f;
        }
    }
    #endregion
}
