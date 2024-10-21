using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenUIManager : MonoBehaviour
{
    //Define Singleton instance
    public static DeathScreenUIManager deathScreenUIManagerInstance { get; private set; }

    //Reference to GameManager
    private GameManager gameManager;

    //Create Singleton Instance of the Death Screen UIManager
    private void Awake()
    {
        if (deathScreenUIManagerInstance == null)
        {
            deathScreenUIManagerInstance = this;
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


    //Button onClick Function
    public void OnButtonClicked(string buttonName)
    {
        switch (buttonName)
        {
            //Return to main menu
            case "MainMenuButton":
            gameManager.sceneLoader("01MainMenu");
            break;

            //Restart Game Level
            case "RestartButton":
            gameManager.sceneLoader("02GameScene");
            break;
        }
    }
}
