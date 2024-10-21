using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProGenManager : MonoBehaviour
{
    //Define Singleton instance
    public static ProGenManager proGenManagerInstance { get; private set; }

    [Header("Plane Spawning Settings")]
    public GameObject[] corridorSections;
    public GameObject[] obstacleSections;
    public float spawnDistance = 5f;

    //List to manage corridor sections
    private List<GameObject> activeCorridors = new List<GameObject>();
    private Transform playerTransform;


    void Start()
    {
        //Create Singleton Instance of the ProGenManager
        if (proGenManagerInstance == null)
        {
            proGenManagerInstance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        //Spawn Initial corridor sections
        SpawnCorridor(spawnDistance);
        SpawnCorridor(spawnDistance * 2);
    }


    public void SpawnCorridor(float positionZ)
    {
        //Determine if an obstacle or corridor section should be spawned
        GameObject newCorridor = InstantiateCorridorOrObstacle(positionZ);
        
        if (newCorridor == null)
        {
            Debug.LogError("Failed to instantiate the plane prefab!");
            return;
        }

        //Add the new corridor/obstacle section to the list
        activeCorridors.Add(newCorridor);

        //Remove the oldest corridor section if there are more than 3 active ones
        if (activeCorridors.Count > 3)
        {
            RemoveOldestCorridor();
        }
    }


    private GameObject InstantiateCorridorOrObstacle(float positionZ)
    {
        int obstacleProbability = Random.Range(1, 10);
        int randomIndex;

        //If probability is greater than or equal to 3, spawn a corridor, otherwise spawn an obstacle
        if (obstacleProbability >= 3)
        {
            randomIndex = Random.Range(0, corridorSections.Length);
            return Instantiate(corridorSections[randomIndex], new Vector3(0, 0, positionZ), Quaternion.identity);
        }
        else
        {
            randomIndex = Random.Range(0, obstacleSections.Length);
            return Instantiate(obstacleSections[randomIndex], new Vector3(0, 0, positionZ), Quaternion.identity);
        }
    }


    private void RemoveOldestCorridor()
    {
        //Get and destroy the oldest corridor/obstacle (first in the list)
        GameObject oldestCorridor = activeCorridors[0];
        activeCorridors.RemoveAt(0);
        Destroy(oldestCorridor);
    }
}
