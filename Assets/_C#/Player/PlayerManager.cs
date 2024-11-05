using UnityEngine;
using System.Collections;

/// <summary>
/// Core player management system handling initialization and references
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
