using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    //Define Singleton instance
    public static MainMenuUIManager mainMenuUIManagerInstance { get; private set; }

    //Reference to GameManager
    private GameManager gameManager;

    //Reference to Main Menu UI elements
    [SerializeField]private GameObject mainMenuPanel;
    [SerializeField]private GameObject optionsPanel;
    [SerializeField]private GameObject helpPanel;


    //Create Singleton Instance of the Main Menu UIManager
    private void Awake()
    {
        if (mainMenuUIManagerInstance == null)
        {
            mainMenuUIManagerInstance = this;
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

        //Enable & Disable UI elements on Start
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        helpPanel.SetActive(false);
    }


    //Button onClick Function
    public void OnButtonClicked(string buttonName)
    {
        switch (buttonName)
        {
            //Start game
            case "StartButton":
            gameManager.sceneLoader("02GameScene");
            break;

            //Quit game
            case "QuitButton":
            Debug.Log("Quit Game");
            Application.Quit();
            break;

            //Show options panel
            case "OptionsButton":
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(true);
            break;

            //Show help panel
            case "HelpButton":
            mainMenuPanel.SetActive(false);
            helpPanel.SetActive(true);
            break;

            //Return to main menu
            case "MainMenuButton":
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
            helpPanel.SetActive(false);
            break;
        }
    }
}
