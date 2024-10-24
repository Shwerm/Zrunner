using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private GameObject player;
    //References
    private PlayerManager playerManager;

    void Awake()
    {
        //Assign the instances
        playerManager = PlayerManager.Instance;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        if (playerManager == null)
        {
            Debug.LogError("PlayerManager instance is not assigned!");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProGenManager spawner = ProGenManager.Instance;

            //Trigger the next corridor to spawn
            if (spawner != null)
            {
                spawner.SpawnCorridor();
                playerManager.ReverseDodge();
            }
            else
            {
                Debug.LogError("ProGenManager instance is not assigned!");
            }
        }
    }
}
