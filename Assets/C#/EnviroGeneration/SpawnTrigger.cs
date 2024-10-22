using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProGenManager spawner = ProGenManager.proGenManagerInstance;

            //Trigger the next corridor to spawn
            if (spawner != null)
            {
                spawner.SpawnCorridor();
            }
            else
            {
                Debug.LogError("ProGenManager instance is not assigned!");
            }
        }
    }
}
