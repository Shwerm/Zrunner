using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TimeManager class is responsible for managing the time in the game
/// and updating difficulty based on player's survival time.
/// Dependencies: GameManager.cs
/// </summary>
public class TimeManager : MonoBehaviour
{
    #region Singleton
    public static TimeManager Instance { get; private set; }
    #endregion

    //Reference to GameManager
    private GameManager gameManager;

    //Player's survival time
    public float playerSurvivalTime = 0f;


    //Create Singleton Instance of the TimeManagerference
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


    void Start()
    {
        //Get reference to GameManager
        gameManager = GameManager.Instance;
    }
    

    void Update()
    {
        playerSurvivalTime += Time.deltaTime;
        gameManager.playerScore = playerSurvivalTime * 10;
    }
}
