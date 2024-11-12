using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Manages the UI elements for the main menu.
/// Dependencies: GameManager.cs
/// </summary>
public class MainMenuUIManager : MonoBehaviour
{
    #region Singleton
    public static MainMenuUIManager Instance { get; private set; }
    #endregion

    //Reference to GameManager
    private GameManager gameManager;

    #region Serialized Fields
    [SerializeField]private GameObject mainMenuPanel;
    [SerializeField]private GameObject optionsPanel;
    [SerializeField]private GameObject helpPanel;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    #endregion


    /// <summary>
    /// Initializes the MainMenuUIManager instance.
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
    /// Assigns the GameManager reference.
    /// Set UI elements to default state.
    /// </summary>
    void Start()
    {
        //Get reference to GameManager
        gameManager = GameManager.Instance;

        //Enable & Disable UI elements on Start
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        helpPanel.SetActive(false);

        //Initialize volume sliders
        if (musicSlider != null)
        {
            musicSlider.value = gameManager.musicVol;
        }
        else
        {
            Debug.LogError("[MainMenuUIManager] Music slider not assigned!");
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = gameManager.sfxVol;
        }
        else
        {
            Debug.LogError("[MainMenuUIManager] SFX slider not assigned!");
        }
    }


    /// <summary>
    /// Handles button clicks for the main menu.
    /// </summary>
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

    public void OnSliderValueChanged(string sliderName)
    {
        switch (sliderName)
        {
            case "MusicSlider":
            gameManager.musicVol = musicSlider.value;
            Debug.Log(gameManager.musicVol);
            break;

            case "SfxSlider":
            gameManager.sfxVol = sfxSlider.value;
            Debug.Log(gameManager.sfxVol);
            break;

            default:
            Debug.Log("Slider value not recognized.");
            break;
        }
    }
}
