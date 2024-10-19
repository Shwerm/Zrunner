using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProGenManager : MonoBehaviour
{
    //Define Singleton instance
    public static ProGenManager proGenManagerInstance { get; private set; }

    [Header("Plane Spawning Settings")]
    public GameObject[] corridorSections; 
    public float spawnDistance = 5f;

    //List to manage planes
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

        SpawnCorridor(spawnDistance);
        SpawnCorridor(spawnDistance * 2);

    }


    public void SpawnCorridor(float positionZ)
    {
        //Pick random corridor section from the array
        int randomIndex = Random.Range(0, corridorSections.Length);

        //Instantiate a new corridor section at the specified Z position
        GameObject newCorridor = Instantiate(corridorSections[randomIndex], new Vector3(0, 0, positionZ), Quaternion.identity);
        if (newCorridor == null)
        {
            Debug.LogError("Failed to instantiate the plane prefab!");
            return;
        }

        //Add the new plane to the list
        activeCorridors.Add(newCorridor);

        //Remove the oldest plane if there are more than 3 planes
        if (activeCorridors.Count > 3)
        {
            //Get the oldest plane (the first in the list)
            GameObject oldestCorridor = activeCorridors[0];
            //Remove it from the list
            activeCorridors.RemoveAt(0);
            //Destroy the oldest plane
            Destroy(oldestCorridor);
        }
    }
}
