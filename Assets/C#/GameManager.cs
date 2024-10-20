using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Define singleton instance of the game manager
    public static GameManager gameManagerInstance;

    public GameObject player;

    public float playerScore;
    public float playerHighScore;

    // Start is called before the first frame update
    void Awake()
    {
        //Create singleton instance of the game manager
        if(gameManagerInstance == null)
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
        //Load main menu scene
        sceneLoader("01MainMenu");
    }

    
    public void sceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}
