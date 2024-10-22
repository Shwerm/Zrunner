using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    // Define Singleton instance
    public static QTEManager QTEManagerInstance { get; private set; }

    private GameSceneUIManager gameSceneUIManager;

    void Awake()
    {
        // Create Singleton Instance of the QTEManager
        if (QTEManagerInstance == null)
        {
            QTEManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }

        gameSceneUIManager = GameSceneUIManager.gameSceneUIManagerInstance;
    }


    public void qteStart()
    {
        Time.timeScale = 0.2f;
        gameSceneUIManager.qteVisualTrigger();
    }

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
