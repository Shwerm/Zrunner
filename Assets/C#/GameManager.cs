using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    //Define singleton instance of the game manager
    public static GameManager gameManagerInstance;

    [Header("Player Variables")]
    public GameObject player;
    public float playerScore;
    public float playerHighScore;

    //File paths
    private string basePath;
    private string highScorePath;
    private string jsonFilePath;


    void Awake()
    {
        // Create singleton instance of the game manager
        if (gameManagerInstance == null)
        {
            gameManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        //Load high score when the game starts (e.g., in the main menu)
        LoadHighScore();

        //Load main menu scene
        sceneLoader("01MainMenu");
    }


    public void sceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void LoadHighScore()
    {
        //Set paths for Program Files and subdirectories
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Zrunner");
        highScorePath = Path.Combine(basePath, "HighScore");
        jsonFilePath = Path.Combine(highScorePath, "playerHighScore.json");

        //Check if the Zrunner directory exists
        if (!Directory.Exists(basePath))
        {
            //If it doesn't exist, create Zrunner and HighScore directories
            Directory.CreateDirectory(highScorePath);

            //Initialize the JSON file with a default high score value of 0
            HighScoreData initialData = new HighScoreData { playerHighScore = 0 };
            string json = JsonUtility.ToJson(initialData, true);

            //Create the JSON file and write the initial high score
            File.WriteAllText(jsonFilePath, json);

            //Set playerHighScore to 0 (initial value)
            playerHighScore = 0;

            Debug.Log("Initialized HighScore file with playerHighScore = 0");
        }
        else
        {
            //If the directory and file already exist, read the JSON file
            if (File.Exists(jsonFilePath))
            {
                //Read the JSON file
                string json = File.ReadAllText(jsonFilePath);

                //Deserialize the JSON data into HighScoreData object
                HighScoreData loadedData = JsonUtility.FromJson<HighScoreData>(json);

                //Set playerHighScore to the value saved in the JSON
                playerHighScore = loadedData.playerHighScore;

                Debug.Log("Loaded playerHighScore: " + playerHighScore);
            }
            else
            {
                Debug.LogError("HighScore JSON file not found!");
            }
        }
    }


    public void SaveNewHighScore()
    {
        //Check if playerScore is higher than playerHighScore
        if (playerScore > playerHighScore)
        {
            //Update playerHighScore to the new value
            playerHighScore = playerScore;

            //Create the data structure to store the new high score
            HighScoreData newHighScoreData = new HighScoreData { playerHighScore = playerHighScore };

            //Convert to JSON format
            string json = JsonUtility.ToJson(newHighScoreData, true);

            //Overwrite the existing JSON file with the new high score
            File.WriteAllText(jsonFilePath, json);

            Debug.Log("New high score saved: " + playerHighScore);
        }
        else
        {
            Debug.Log("Current score (" + playerScore + ") is less than high score (" + playerHighScore + "). No update.");
        }
    }


    public void playerDeath()
    {
        sceneLoader("03PlayerDeathScene");
        SaveNewHighScore();
    }
}


//High score data class
[System.Serializable]
public class HighScoreData
{
    public float playerHighScore = 0;
}
