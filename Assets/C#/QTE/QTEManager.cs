using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages the QTE system in the game.
/// Dependencies: GameSceneUIManager.cs, PlayerManager.cs, PlayerCameraManager.cs
/// </summary>
public class QTEManager : MonoBehaviour
{
    #region Singleton
    public static QTEManager Instance { get; private set; }
    #endregion

    #region Private Fields
    private GameSceneUIManager gameSceneUIManager;
    private PlayerManager playerManager;
    private PlayerCameraManager playerCameraManager;
    #endregion


    /// <summary>
    /// Initializes the QTEManager instance.
    /// </summary>
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
    }


    /// <summary>
    /// Assigns the instances of the GameSceneUIManager, PlayerManager, and PlayerCameraManager.
    /// </summary>
    void Start()
    {
        //Assign the instances
        gameSceneUIManager = GameSceneUIManager.gameSceneUIManagerInstance;
        playerManager = PlayerManager.Instance;
        playerCameraManager = PlayerCameraManager.Instance;

        //Error Handling
        if(gameSceneUIManager == null)
        {
            Debug.LogError("[QTEManager] GameSceneUIManager instance is not assigned!");
        }

        if(playerManager == null)
        {
            Debug.LogError("[QTEManager] PlayerManager instance is not assigned!");
        }

        if(playerCameraManager == null)
        {
            Debug.LogError("[QTEManager] PlayerCameraManager instance is not assigned!");
        }
    }


    /// <summary>
    /// Starts the QTE process.
    /// </summary>
    public void qteStart()
    {
        Time.timeScale = 0.2f;
        gameSceneUIManager.qteVisualTrigger();
    }


    /// <summary>
    /// Switch statement to handle the QTE success.
    /// Takes in the active QTE as a string paraameter.
    /// </summary>
    /// <param name="activeQTE"></param> 
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
            playerCameraManager.LookLeft();
            break;

            case "DodgeLeft":
            Time.timeScale = 1f;
            Debug.Log("DodgeLeft QTE");
            playerManager.Dodge(-4);
            playerCameraManager.LookRight();
            break;

            default:
            Debug.Log("Invalid QTE Type");
            break;
        }
    }
}
