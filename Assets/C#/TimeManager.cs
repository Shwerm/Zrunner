using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //Define Sinngleton instance
    public static TimeManager timeManagerInstance { get; private set; }

    //Reference to GameManager
    private GameManager gameManager;

    public float playerSurvivalTime = 0f;


    //Create Singleton Instance of the TimeManager
    private void Awake()
    {
        if (timeManagerInstance == null)
        {
            timeManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        //Get reference to GameManager
        gameManager = GameManager.gameManagerInstance;
    }
    

    void Update()
    {
        playerSurvivalTime += Time.deltaTime;
        gameManager.playerScore = playerSurvivalTime * 10;
    }
    
}
