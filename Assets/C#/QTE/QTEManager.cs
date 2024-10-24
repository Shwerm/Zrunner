using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    //Define Singleton instance
    public static QTEManager Instance { get; private set; }

    //References
    private GameSceneUIManager gameSceneUIManager;
    private PlayerManager playerManager;


    void Awake()
    {
        //Create Singleton Instance of the QTEManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //Assign the instances
        gameSceneUIManager = GameSceneUIManager.gameSceneUIManagerInstance;
        playerManager = PlayerManager.Instance;

        //Error Handling
        if(gameSceneUIManager == null)
        {
            Debug.LogError("GameSceneUIManager instance is not assigned!");
        }

        if(playerManager == null)
        {
            Debug.LogError("PlayerManager instance is not assigned!");
        }
    }


    //QTE Start
    public void qteStart()
    {
        Time.timeScale = 0.2f;
        gameSceneUIManager.qteVisualTrigger();
    }


    //QTE Success
    public void qteSuccess(string activeQTE)
    {
        switch (activeQTE)
        {
            case "Jump":
            Debug.Log("Jump QTE");
            playerManager.Jump();
            break;

            case "Slide":
            Time.timeScale = 1f;
            Debug.Log("Slide QTE");
            break;

            case "DodgeRight":
            Time.timeScale = 1f;
            Debug.Log("DodgeRight QTE");
            playerManager.Dodge(4);
            playerManager.LookLeft();
            break;

            case "DodgeLeft":
            Time.timeScale = 1f;
            Debug.Log("DodgeLeft QTE");
            playerManager.Dodge(-4);
            playerManager.LookRight();
            break;

            default:
            Debug.Log("Invalid QTE Type");
            break;
        }
    }
}
