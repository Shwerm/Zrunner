using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// Manages the UI for the death screen
/// Dependencies: GameManager.cs
/// </summary>
public class DeathScreenUIManager : MonoBehaviour
{
    #region Singleton
    public static DeathScreenUIManager Instance { get; private set; }
    #endregion

    #region Private Fields
    private GameManager gameManager;

    [Header("UI Score Elements")]
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScoreText;
    #endregion


    /// <summary>
    /// Initializes the DeathScreenUIManager instance.
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
    /// Assigns the GameManager instance to the private field
    /// </summary>
    void Start()
    {
        //Get reference to GameManager
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("[DeathScreenUIManager] GameManager instance is not assigned!");
        }
        else
        {
            //Set the current score text
            currentScoreText.text = " Your Score: " + gameManager.playerScore.ToString();
            //Set the high score text
            highScoreText.text = "High Score: " + gameManager.playerHighScore.ToString();
        }


    }


    /// <summary>
    /// Handles button clicks on the death screen
    /// </summary>
    /// <param name="buttonName"></param>
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

            default:
            Debug.LogError("[DeathScreenUIManager] Button name is not assigned!");
            break;
        }
    }
}
