using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            //Spawn the next plane
            ProGenManager spawner = FindObjectOfType<ProGenManager>();
            if (spawner != null)
            {
                //Spawn the next plane in front of the player
                spawner.SpawnCorridor(transform.position.z + spawner.spawnDistance);
            }
        }
    }
    
}
