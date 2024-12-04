using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;


/// <summary>
/// Manages the game state and persistent game data.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Public Fields
    [Header("Player Variables")]
    public float playerScore;
    public float playerHighScore;

    public float musicVol = 5;
    public float sfxVol = 5;
    #endregion

    #region Private Fields
    //File paths
    private string basePath;
    private string highScorePath;
    private string jsonFilePath;
    #endregion


    /// <summary>
    /// Initializes the GameManager instance.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Loads the high score from the JSON file.
    /// Loads main menu scene.
    /// </summary>
    void Start()
    {
        //Load high score when the game starts (e.g., in the main menu)
        LoadHighScore();

        //Load main menu scene
        sceneLoader("01MainMenu");
    }


    /// <summary>
    /// Public scene loader function.
    /// </summary>
    public void sceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    /// <summary>
    /// Checks if game file exists.
    /// Creates game file if it doesn't exist.
    /// Creates high score JSON file if it doesn't exist.
    /// Loads high score from the JSON file.
    /// </summary>
    public void LoadHighScore()
    {
        //Set paths for Program Files and subdirectories
        basePath = Path.Combine(Application.persistentDataPath, "Zrunner");
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

            Debug.Log("[GameManager] Initialized HighScore file with playerHighScore = 0");
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

                Debug.Log("[GameManager] Loaded playerHighScore: " + playerHighScore);
            }
            else
            {
                Debug.LogError("[GameManager] HighScore JSON file not found!");
            }
        }
    }


    /// <summary>
    /// Saves the new high score to the JSON file if it's higher than the current high score.
    /// </summary>
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

            Debug.Log("[GameManager] New high score saved: " + playerHighScore);
        }
        else
        {
            Debug.Log("[GameManager] Current score (" + playerScore + ") is less than high score (" + playerHighScore + "). No update.");
        }
    }


    /// <summary>
    /// Loads the main menu scene on player death.
    /// </summary>
    public void playerDeath()
    {
        sceneLoader("03PlayerDeathScene");
        SaveNewHighScore();
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "02GameScene")
        {
            time.timeScale = 1f;
        }
    }
}


/// <summary>
/// Data structure to store the high score.
/// </summary>
[System.Serializable]
public class HighScoreData
{
    public float playerHighScore = 0;
}
