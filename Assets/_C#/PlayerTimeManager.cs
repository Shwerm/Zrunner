using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TimeManager class is responsible for managing the time in the game
/// and updating difficulty based on player's survival time.
/// Dependencies: GameManager.cs
/// </summary>
public class PlayerTimeManager : MonoBehaviour
{
    #region Singleton
    public static PlayerTimeManager Instance { get; private set; }
    #endregion

    //Reference to GameManager
    private GameManager gameManager;

    //Player's survival time
    public float playerSurvivalTime = 0f;


    /// <summary>
    /// Initializes the TimeManager instance.
    /// </summary>
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


    /// <summary>
    /// Gets reference to GameManager.
    /// </summary>
    void Start()
    {
        //Get reference to GameManager
        gameManager = GameManager.Instance;
    }
    

    /// <summary>
    /// Updates player's survival time and game manager's score.
    /// </summary>
    void Update()
    {
        playerSurvivalTime += Time.deltaTime;
        gameManager.playerScore = playerSurvivalTime * 10;
    }
}
