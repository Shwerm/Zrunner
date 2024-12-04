using UnityEngine;
using System.Collections;

/// <summary>
/// Core player management system combining player components and state.
/// Provides centralised access to player subsystems and handles initialization.
/// 
/// Dependencies: GameManager.cs, ParkourQTEManager.cs, PlayerCameraManager.cs, GameSceneUIManager.cs
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance { get; private set; }
    #endregion

    #region Components
    public PlayerMovement Movement { get; private set; }
    public PlayerCollisionHandler CollisionHandler { get; private set; }
    #endregion

    #region Private Fields
    private GameManager gameManager;
    private ParkourQTEManager parkourQTEManager;
    private PlayerCameraManager playerCameraManager;
    private GameSceneUIManager gameSceneUIManager;
    #endregion

    #region QTE State
    public string activeParkourQTE;
    public string activeCombatQTE;
    #endregion

    #region Initialization
    private void Awake()
    {
        InitializeSingleton();
        InitializeComponents();
    }

    /// <summary>
    /// Initializes singleton instance and validates core dependencies.
    /// Sets up component references and manager connections.
    /// </summary>
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitializeComponents()
    {
        Movement = GetComponent<PlayerMovement>();
        CollisionHandler = GetComponent<PlayerCollisionHandler>();
    }

    void Start()
    {
        AssignManagerReferences();
        ValidateManagerReferences();
    }

    private void AssignManagerReferences()
    {
        gameManager = GameManager.Instance;
        parkourQTEManager = ParkourQTEManager.Instance;
        playerCameraManager = PlayerCameraManager.Instance;
        gameSceneUIManager = GameSceneUIManager.Instance;
    }

    /// <summary>
    /// Validates critical manager references.
    /// Logs errors for missing dependencies to aid debugging.
    /// </summary>
    private void ValidateManagerReferences()
    {
        if (gameManager == null) Debug.LogError("[PlayerManager] Game Manager is not assigned!");
        if (parkourQTEManager == null) Debug.LogError("[PlayerManager] QTE Manager is not assigned!");
        if (playerCameraManager == null) Debug.LogError("[PlayerManager] Player Camera Manager is not assigned!");
        if (gameSceneUIManager == null) Debug.LogError("[PlayerManager] Game Scene UI Manager is not assigned!");
    }
    #endregion

    #region Event Subscriptions
    private void OnEnable()
    {
        StartCoroutine(SubscribeToTimeManager());
    }

    /// <summary>
    /// Sets up time manager event subscriptions.
    /// Ensures proper timing for difficulty-based speed updates.
    /// </summary>
    private IEnumerator SubscribeToTimeManager()
    {
        yield return new WaitForEndOfFrame();
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDifficultyChanged += Movement.UpdatePlayerSpeed;
        }
    }
    
    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDifficultyChanged -= Movement.UpdatePlayerSpeed;
    }
    #endregion
}