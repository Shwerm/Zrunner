using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProGenManager : MonoBehaviour
{
    // Define Singleton instance
    public static ProGenManager proGenManagerInstance { get; private set; }

    [Header("Plane Spawning Settings")]
    public GameObject[] corridorSections;
    public GameObject[] obstacleSections;
    public float spawnDistance = 5f;

    // List to manage corridor sections
    private List<GameObject> activeCorridors = new List<GameObject>();

    // Reference to the manually placed corridor section
    public GameObject initialCorridorSection;

    private float nextSpawnZ = 0f;

    void Start()
    {
        // Create Singleton Instance of the ProGenManager
        if (proGenManagerInstance == null)
        {
            proGenManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }

        // Ensure the manually placed section is tracked
        TrackInitialCorridor();

        // Spawn additional initial corridor sections with correct spacing
        SpawnInitialCorridors(4); // Adjusted to spawn 4 sections since 1 is manually placed
    }

    void TrackInitialCorridor()
    {
        if (initialCorridorSection != null)
        {
            // Add the manually placed section to the list and set the next spawn position after it
            activeCorridors.Add(initialCorridorSection);

            // Set nextSpawnZ based on the initial corridor's Z position + spawn distance
            nextSpawnZ = initialCorridorSection.transform.position.z + spawnDistance;
        }
        else
        {
            Debug.LogError("Initial corridor section is not assigned!");
        }
    }

    void SpawnInitialCorridors(int numberOfSections)
    {
        // Spawn the specified number of corridor sections initially
        for (int i = 0; i < numberOfSections; i++)
        {
            SpawnCorridor();
        }
    }


    //Wait two seconds coroutine definition
    IEnumerator StreamChunkUnload()
    {
        yield return new WaitForSeconds(3);
        GameObject oldestCorridor = activeCorridors[0];
        activeCorridors.RemoveAt(0);
        Destroy(oldestCorridor);
    }

    public void SpawnCorridor()
    {
        // Spawn the corridor at the next Z position
        int randomIndex = Random.Range(0, corridorSections.Length);
        GameObject newCorridor = Instantiate(corridorSections[randomIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);

        if (newCorridor == null)
        {
            Debug.LogError("Failed to instantiate the plane prefab!");
            return;
        }

        // Add the new plane to the list
        activeCorridors.Add(newCorridor);

        // Increment the next spawn position
        nextSpawnZ += spawnDistance;

        // Remove the oldest plane only when there are more than 5 active planes
        if (activeCorridors.Count > 5) // Adjust this number as needed
        {
            StartCoroutine(StreamChunkUnload());
        }
    }
}
