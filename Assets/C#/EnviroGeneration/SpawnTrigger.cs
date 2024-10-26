using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///Handles the triggering of new corridor/obstacle section spawns and player position resets
///Attached to trigger volumes at the end of each section
///Dependencies: PlayerManager.cs, ProGenManager.cs
///</summary>
public class SpawnTrigger : MonoBehaviour
{
    #region Private Fields
    private GameObject player;
    private PlayerManager playerManager;
    private ProGenManager spawner;
    #endregion


    ///<summary>
    ///Initialize references and perform error checking on startup
    ///</summary>
    void Start()
    {
        //Assign the singleton instance reference
        playerManager = PlayerManager.Instance;
        spawner = ProGenManager.Instance;

        //Locate and validate required game objects and components
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[SpawnTrigger] Player not found!");
        }

        if (playerManager == null)
        {
            Debug.LogError("PlayerManager instance is not assigned!");
        }
    }


    ///<summary>
    ///Detects when player enters the trigger volume
    ///Spawns next section and resets player position
    ///</summary>
    ///<param name="other">The collider that entered the trigger volume</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Generate next section and reset player position
            if (spawner != null)
            {
                spawner.SpawnCorridor();
                playerManager.ReverseDodge();
            }
            else
            {
                Debug.LogError("[SpawnTrigger] ProGenManager instance is not assigned!");
            }
        }
    }
}