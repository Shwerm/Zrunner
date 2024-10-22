using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the next corridor to spawn earlier, when the player is in the middle of the section
            ProGenManager spawner = ProGenManager.proGenManagerInstance;

            if (spawner != null)
            {
                spawner.SpawnCorridor();
            }
        }
    }
}
