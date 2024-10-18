using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Define singleton instance of the game manager
    public static GameManager gameManagerInstance;


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

    void sceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
