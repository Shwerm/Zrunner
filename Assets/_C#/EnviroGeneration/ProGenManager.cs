using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Manages procedural generation of the game environment including corridors and obstacles.
///Implements object pooling and event-driven architecture for optimal performance.
///Dependencies: ProGenConfig.cs
///</summary>
public class ProGenManager : MonoBehaviour
{
    #region Singleton
    public static ProGenManager Instance { get; private set; }
    #endregion

    #region Events
    public event EventHandler<SectionSpawnedEventArgs> OnSectionSpawned;
    #endregion

    #region Serialized Fields
    [Header("Configuration")]
    [SerializeField] private ProGenConfig config;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject initialCorridorSection;
    [SerializeField] private GameObject[] corridorSections;
    [SerializeField] private GameObject[] obstacleSections;
    [SerializeField] private GameObject[] enemySections;
    #endregion

    #region Private Fields
    private Dictionary<SectionType, Queue<GameObject>> objectPools;
    private List<GameObject> activeCorridors = new List<GameObject>();
    private float nextSpawnZ;
    private Coroutine streamChunkCoroutine;
    #endregion

    #region Initialization
    ///<summary>
    ///Initializes the ProGenManager instance and object pools
    ///</summary>
    private void Awake()
    {
        InitializeSingleton();
        InitializeObjectPools();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeObjectPools()
    {
        objectPools = new Dictionary<SectionType, Queue<GameObject>>();
        CreatePool(SectionType.Corridor, corridorSections);
        CreatePool(SectionType.Obstacle, obstacleSections);
        CreatePool(SectionType.Enemy, enemySections);
    }

    private void CreatePool(SectionType type, GameObject[] prefabs)
    {
        var pool = new Queue<GameObject>();
        foreach (var prefab in prefabs)
        {
            for (int i = 0; i < config.PoolSizePerSection; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        objectPools[type] = pool;
    }
    #endregion

    #region Core Generation Logic
    ///<summary>
    ///Initializes the corridor generation system and spawns initial sections
    ///</summary>
    private void Start()
    {
        ValidateConfiguration();
        TrackInitialCorridor();
        SpawnInitialCorridors(config.InitialCorridorCount);
    }

    private void ValidateConfiguration()
    {
        if (config == null)
        {
            Debug.LogError("[ProGenManager] Configuration asset is missing!");
            enabled = false;
            return;
        }
    }

    private void TrackInitialCorridor()
    {
        if (initialCorridorSection != null)
        {
            activeCorridors.Add(initialCorridorSection);
            nextSpawnZ = initialCorridorSection.transform.position.z + config.SpawnDistance;
        }
        else
        {
            Debug.LogError("[ProGenManager] Initial corridor section is not assigned!");
        }
    }

    private void SpawnInitialCorridors(int numberOfSections)
    {
        for (int i = 0; i < numberOfSections; i++)
        {
            SpawnCorridor();
        }
    }
    #endregion

    #region Section Management
    ///<summary>
    ///Spawns a new section based on probability configuration
    ///Uses object pooling for optimal performance
    ///</summary>
    public void SpawnCorridor()
    {
        try
        {
            int randomNum = UnityEngine.Random.Range(1, 13);
            GameObject newSection = GetNextSection(randomNum);
            
            if (newSection != null)
            {
                PositionSection(newSection);
                TrackNewSection(newSection);
                RaiseSpawnEvent(newSection);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[ProGenManager] Failed to spawn section: {e.Message}");
        }
    }

    private GameObject GetNextSection(int randomNum)
    {
        SectionType sectionType = DetermineSectionType(randomNum);
        return GetObjectFromPool(sectionType);
    }

    private SectionType DetermineSectionType(int randomNum)
    {
        if (randomNum >= config.EnemySpawnChanceThreshold)
            return SectionType.Enemy;
        if (randomNum >= config.ObstacleSpawnChanceThreshold)
            return SectionType.Corridor;
        return SectionType.Obstacle;
    }

    private GameObject GetObjectFromPool(SectionType type)
    {
        if (objectPools[type].Count > 0)
        {
            var obj = objectPools[type].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        Debug.LogWarning($"[ProGenManager] Pool depleted for {type}");
        return null;
    }

    private void PositionSection(GameObject section)
    {
        section.transform.position = new Vector3(0, 0, nextSpawnZ);
        nextSpawnZ += config.SpawnDistance;
    }

    private void TrackNewSection(GameObject section)
    {
        activeCorridors.Add(section);
        if (activeCorridors.Count > config.MaxActiveCorridors)
        {
            streamChunkCoroutine = StartCoroutine(StreamChunkUnload());
        }
    }

    private void RaiseSpawnEvent(GameObject section)
    {
        OnSectionSpawned?.Invoke(this, new SectionSpawnedEventArgs
        {
            SpawnedSection = section,
            SpawnPosition = section.transform.position
        });
    }
    #endregion

    #region Cleanup
    ///<summary>
    ///Manages the cleanup of old sections using object pooling
    ///</summary>
    private IEnumerator StreamChunkUnload()
    {
        yield return new WaitForSeconds(config.ChunkUnloadDelay);
        
        if (activeCorridors.Count > 0)
        {
            GameObject oldestCorridor = activeCorridors[0];
            activeCorridors.RemoveAt(0);
            ReturnObjectToPool(oldestCorridor);
        }
    }

    private void ReturnObjectToPool(GameObject section)
    {
        section.SetActive(false);
        SectionType type = DetermineSectionType(section);
        objectPools[type].Enqueue(section);
    }

    private SectionType DetermineSectionType(GameObject section)
    {
        // Determine section type based on tag or component
        return SectionType.Corridor; // Default fallback
    }
    #endregion
}

#region Supporting Types
public class SectionSpawnedEventArgs : EventArgs
{
    public GameObject SpawnedSection { get; set; }
    public Vector3 SpawnPosition { get; set; }
}

public enum SectionType
{
    Corridor,
    Obstacle,
    Enemy
}
#endregion