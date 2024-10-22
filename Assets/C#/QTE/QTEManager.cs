using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    //Define Singleton instance
    public static QTEManager QTEManagerInstance { get; private set; }

    //Reference to the GameSceneUIManager
    private GameSceneUIManager gameSceneUIManager;


    void Awake()
    {
        //Create Singleton Instance of the QTEManager
        if (QTEManagerInstance == null)
        {
            QTEManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }

        //Assign the GameSceneUIManager instance
        gameSceneUIManager = GameSceneUIManager.gameSceneUIManagerInstance;

        if(gameSceneUIManager == null)
        {
            Debug.LogError("GameSceneUIManager instance is not assigned!");
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
            break;

            case "Slide":
            Debug.Log("Slide QTE");
            break;

            case "DodgeRight":
            Debug.Log("DodgeRight QTE");
            break;

            case "DodgeLeft":
            Debug.Log("DodgeLeft QTE");
            break;

            default:
            Debug.Log("Invalid QTE Type");
            break;
        }
    }
}
