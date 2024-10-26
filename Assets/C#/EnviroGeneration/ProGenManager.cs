using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///Manages procedural generation of the game environment including corridors and obstacles.
///Dependencies: N/A
///</summary>
public class ProGenManager : MonoBehaviour
{
    #region Singleton
    public static ProGenManager Instance { get; private set; }
    #endregion


    #region Serialized Fields
    [Header("Generation Settings")]
    [SerializeField] private int initialCorridorCount = 4;
    [SerializeField] private int maxActiveCorridors = 5;
    [SerializeField] private float chunkUnloadDelay = 3f;
    
    [Header("Spawn Settings")]
    [SerializeField] [Range(1, 10)] private int spawnChanceThreshold = 4;
    [SerializeField] private float spawnDistance = 5f;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject initialCorridorSection;
    [SerializeField] private GameObject[] corridorSections;
    [SerializeField] private GameObject[] obstacleSections;
    #endregion


    #region Private Fields
    private List<GameObject> activeCorridors = new List<GameObject>();
    private float nextSpawnZ;
    private Coroutine streamChunkCoroutine;
    #endregion


    ///<summary>
    ///Initializes the ProGenManager instance.
    ///</summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    ///<summary>
    ///Initializes the corridor generation system
    ///</summary>
    private void Start()
    {
        TrackInitialCorridor();
        SpawnInitialCorridors(initialCorridorCount);
    }


    ///<summary>
    ///Sets up tracking for the manually placed initial corridor section
    ///</summary>
    private void TrackInitialCorridor()
    {
        if (initialCorridorSection != null)
        {
            activeCorridors.Add(initialCorridorSection);
            nextSpawnZ = initialCorridorSection.transform.position.z + spawnDistance;
        }
        else
        {
            Debug.LogError("[ProGenManager] Initial corridor section is not assigned!");
        }
    }


    ///<summary>
    ///Spawns the initial set of corridor sections
    ///</summary>
    ///<param name="numberOfSections">Number of sections to spawn initially</param>
    private void SpawnInitialCorridors(int numberOfSections)
    {
        for (int i = 0; i < numberOfSections; i++)
        {
            SpawnCorridor();
        }
    }


    ///<summary>
    ///<summary>
    ///Coroutine to handle the removal of old corridor sections
    ///</summary>
    private IEnumerator StreamChunkUnload()
    {
        yield return new WaitForSeconds(chunkUnloadDelay);
        GameObject oldestCorridor = activeCorridors[0];
        activeCorridors.RemoveAt(0);
        Destroy(oldestCorridor);
    }


    ///<summary>
    ///Spawns a new corridor or obstacle section based on random probability
    ///70% chance for corridor, 30% chance for obstacle
    ///</summary>
    public void SpawnCorridor()
    {
        int randomNum = Random.Range(1, 10);
        GameObject newSection;

        //70% chance for corridor, 30% chance for obstacle
        if (randomNum >= spawnChanceThreshold)
        {
            int randomIndex = Random.Range(0, corridorSections.Length);
            newSection = Instantiate(corridorSections[randomIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        }
        else
        {
            int randomIndex = Random.Range(0, obstacleSections.Length);
            newSection = Instantiate(obstacleSections[randomIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        }

        if (newSection == null)
        {
            Debug.LogError("[ProGenManager] Failed to instantiate the section prefab!");
            return;
        }

        activeCorridors.Add(newSection);
        nextSpawnZ += spawnDistance;

        //Maintain maximum of 5 active sections
        if (activeCorridors.Count > maxActiveCorridors)
        {
            StartCoroutine(StreamChunkUnload());
        }
    }
}
