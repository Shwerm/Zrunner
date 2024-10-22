using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    // Define Singleton instance
    public static QTEManager QTEManagerInstance { get; private set; }

    private GameSceneUIManager gameSceneUIManager;

    void Start()
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


    public void JumpQTE()
    {
        Debug.Log("Jump QTE");
        Time.timeScale = 0.5f;
        gameSceneUIManager.qteVisualTrigger();
    }

    public void SlideQTE()
    {
        Debug.Log("Slide QTE");
        Time.timeScale = 0.5f;
        gameSceneUIManager.qteVisualTrigger();
    }

    public void DodgeRightQTE()
    {
        Debug.Log("Dodge Right QTE");
        Time.timeScale = 0.5f;
        gameSceneUIManager.qteVisualTrigger();
    }

    public void DodgeLeftQTE()
    {
        Debug.Log("Dodge Left QTE");
        Time.timeScale = 0.5f;
        gameSceneUIManager.qteVisualTrigger();
    }
}
