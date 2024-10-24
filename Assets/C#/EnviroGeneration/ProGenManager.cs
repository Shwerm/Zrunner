using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages procedural generation of the game environment including corridors and obstacles.
/// Implements object pooling for efficient resource management.
/// </summary>
public class ProGenManager : MonoBehaviour
{
    public static ProGenManager Instance { get; private set; }

    [SerializeField] private SpawnConfiguration m_Config;

    private const int k_InitialCorridorCount = 4;
    private const int k_MaxActiveCorridors = 5;
    private const float k_UnloadDelay = 3f;
    private const int k_ObstacleSpawnThreshold = 4;
    private const int k_RandomRangeMax = 11;

    private readonly List<GameObject> m_ActiveCorridors = new List<GameObject>();
    private readonly Dictionary<string, Queue<GameObject>> m_CorridorPools = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<string, Queue<GameObject>> m_ObstaclePools = new Dictionary<string, Queue<GameObject>>();
    
    private float m_NextSpawnZ;
    private bool m_IsInitialized;

    private void Awake()
    {
        InitializeSingleton();
        InitializeObjectPools();
        InitializeStartingCorridor();
        SpawnInitialCorridors(k_InitialCorridorCount);
        m_IsInitialized = true;
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeObjectPools()
    {
        foreach (var corridor in m_Config.CorridorSections)
        {
            CreatePool(corridor, 5, m_CorridorPools);
        }

        foreach (var obstacle in m_Config.ObstacleSections)
        {
            CreatePool(obstacle, 3, m_ObstaclePools);
        }
    }

    private void CreatePool(GameObject prefab, int initialSize, Dictionary<string, Queue<GameObject>> poolDictionary)
    {
        var pool = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        poolDictionary[prefab.name] = pool;
    }

    private GameObject GetPooledObject(GameObject prefab, Dictionary<string, Queue<GameObject>> poolDictionary)
    {
        if (!poolDictionary.ContainsKey(prefab.name))
        {
            CreatePool(prefab, 2, poolDictionary);
        }

        var pool = poolDictionary[prefab.name];
        if (pool.Count == 0)
        {
            var obj = Instantiate(prefab);
            pool.Enqueue(obj);
        }

        var pooledObject = pool.Dequeue();
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void SpawnCorridor()
    {
        if (!m_IsInitialized) return;

        int randomNum = UnityEngine.Random.Range(1, k_RandomRangeMax);
        GameObject sectionPrefab;
        GameObject newSection;

        if (randomNum >= k_ObstacleSpawnThreshold)
        {
            sectionPrefab = m_Config.CorridorSections[UnityEngine.Random.Range(0, m_Config.CorridorSections.Length)];
            newSection = GetPooledObject(sectionPrefab, m_CorridorPools);
        }
        else
        {
            sectionPrefab = m_Config.ObstacleSections[UnityEngine.Random.Range(0, m_Config.ObstacleSections.Length)];
            newSection = GetPooledObject(sectionPrefab, m_ObstaclePools);
        }

        newSection.transform.position = new Vector3(0f, 0f, m_NextSpawnZ);
        m_ActiveCorridors.Add(newSection);
        m_NextSpawnZ += m_Config.SpawnDistance;

        if (m_ActiveCorridors.Count > k_MaxActiveCorridors)
        {
            StartCoroutine(UnloadOldestCorridor());
        }
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        if (obj.CompareTag("Corridor"))
        {
            m_CorridorPools[obj.name].Enqueue(obj);
        }
        else
        {
            m_ObstaclePools[obj.name].Enqueue(obj);
        }
    }

    private void OnDestroy()
    {
        m_IsInitialized = false;
        m_CorridorPools.Clear();
        m_ObstaclePools.Clear();
    }
}