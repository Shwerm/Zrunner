using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for (int j = 0; j < prefabs.Length; j++)
        {
            for (int i = 0; i < config.PoolSizePerSection; i++)
            {
                var obj = Instantiate(prefabs[j]);
                obj.name = prefabs[j].name + "_" + i;
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        objectPools[type] = pool;
    }
    #endregion

    #region Core Generation Logic
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
        if (activeCorridors.Count > 0)
        {
            GameObject lastSection = activeCorridors[activeCorridors.Count - 1];
            SectionType lastSectionType = DetermineSectionType(lastSection);
            if (lastSectionType == SectionType.Enemy || lastSectionType == SectionType.Obstacle)
            {
                return GetSectionFromPool(SectionType.Corridor);
            }
        }

        SectionType sectionType = DetermineSectionTypeFromRandom(randomNum);
        return GetSectionFromPool(sectionType);
    }

    private GameObject GetSectionFromPool(SectionType sectionType)
    {
        GameObject[] sectionArray = GetSectionArray(sectionType);
        int randomIndex = UnityEngine.Random.Range(0, sectionArray.Length);
        
        if (objectPools[sectionType].Count == 0)
        {
            var newObject = Instantiate(sectionArray[randomIndex]);
            newObject.name = sectionArray[randomIndex].name + "_new";
            newObject.SetActive(true);
            return newObject;
        }

        var pooledObjects = objectPools[sectionType].ToArray();
        var selectedObject = pooledObjects[UnityEngine.Random.Range(0, pooledObjects.Length)];
        objectPools[sectionType] = new Queue<GameObject>(pooledObjects.Where(x => x != selectedObject));
        
        selectedObject.SetActive(true);
        return selectedObject;
    }

    private GameObject[] GetSectionArray(SectionType sectionType)
    {
        switch (sectionType)
        {
            case SectionType.Corridor:
                return corridorSections;
            case SectionType.Obstacle:
                return obstacleSections;
            case SectionType.Enemy:
                return enemySections;
            default:
                return corridorSections;
        }
    }

    private SectionType DetermineSectionTypeFromRandom(int randomNum)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        
        if (randomValue <= config.CorridorSpawnChance)
        {
            return SectionType.Corridor;
        }
        else if (randomValue <= config.CorridorSpawnChance + config.ObstacleSpawnChance)
        {
            return SectionType.Obstacle;
        }
        else
        {
            return SectionType.Enemy;
        }
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
        if (section.name.Contains("Enemy"))
            return SectionType.Enemy;
        if (section.name.Contains("Obstacle"))
            return SectionType.Obstacle;
        return SectionType.Corridor;
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
