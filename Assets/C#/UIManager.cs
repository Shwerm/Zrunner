using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Define Singleton instance
    public static UIManager uiManagerInstance { get; private set; }

    //Reference to GameManager
    private GameManager gameManager;

    //Create Singleton Instance of the UIManager
    private void Awake()
    {
        if (uiManagerInstance == null)
        {
            uiManagerInstance = this;
            DontDestroyOnLoad(this);
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
            case "StartButton":
            //Load game scene
            Debug.Log("Start Button Clicked");
            gameManager.sceneLoader("02GameScene");
            break;

            //case "QuitButton":
        }
    }
}
