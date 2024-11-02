using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Manages player core functionality including movement, collision handling, and QTE interactions.
/// Implements state-based behavior and event-driven architecture for optimal performance.
/// Dependencies: PlayerConfig.cs, GameManager.cs, QTEManager.cs, PlayerCameraManager.cs
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance { get; private set; }
    #endregion

    #region Events
    public static event Action<PlayerState> OnPlayerStateChanged;
    public static event Action OnPlayerCollision;
    public static event Action OnPlayerJump;
    #endregion

    #region Serialized Fields
    [Header("Configuration")]
    [SerializeField] private PlayerConfig playerConfig;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode;
    #endregion

    #region State Management
    private PlayerState currentState;
    public PlayerState CurrentState 
    {
        get => currentState;
        private set 
        {
            currentState = value;
            OnPlayerStateChanged?.Invoke(currentState);
        }
    }
    #endregion

    #region Private Fields
    private GameManager gameManager;
    private ParkourQTEManager parkourQTEManager;
    private PlayerCameraManager playerCameraManager;
    private GameSceneUIManager gameSceneUIManager;
    
    private Vector3 targetPosition;
    private float originalXPosition;
    
    // Cached components
    private Rigidbody rb;
    private Transform tr;
    #endregion

    #region Public Properties
    public string activeParkourQTE { get; private set; }
    public string activeCombatQTE { get; private set; }
    #endregion

    // Rest of the implementation remains the same, just removing Input System specific code

    #region Initialization
    private void Awake()
    {
        InitializeSingleton();
        CacheComponents();
    }

    private void OnEnable()
    {
        InitializeInput();
        SubscribeToEvents();
    }

    void Start()
    {
        Initialize();
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

    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody>();
        tr = transform;
    }

    private void Initialize()
    {
        originalXPosition = tr.position.x;
        AssignManagerReferences();
        ValidateManagerReferences();
        CurrentState = PlayerState.Running;
    }

    private void InitializeInput()
    {
        moveAction = inputActions.FindAction("Move");
        moveAction.Enable();
    }
    #endregion

    #region Core Gameplay Loop
    private void Update()
    {
        if (CurrentState == PlayerState.Running)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        tr.Translate(Vector3.forward * playerConfig.moveSpeed * Time.deltaTime);
    }
    #endregion
    
    #region Collision Handling
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            OnPlayerCollision?.Invoke();
            gameManager.playerDeath();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        HandleParkourTriggers(other);
        HandleCombatTriggers(other);
    }
    #endregion

    #region Movement Actions
    /// <summary>
    /// Returns player to original X position after dodge
    /// </summary>
    public void ReverseDodge()
    {
        CurrentState = PlayerState.Dodging;
        targetPosition = tr.position;
        targetPosition.x = originalXPosition;
        StartCoroutine(SmoothDodge());
    }

    private IEnumerator SmoothDodge()
    {
        while (tr.position.x != targetPosition.x)
        {
            tr.position = new Vector3(
                Mathf.MoveTowards(tr.position.x, targetPosition.x, playerConfig.dodgeSpeed * Time.deltaTime),
                tr.position.y,
                tr.position.z
            );
            yield return null;
        }
        CurrentState = PlayerState.Running;
    }

    public void Jump()
    {
        CurrentState = PlayerState.Jumping;
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * playerConfig.jumpForce);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        HandleJumpCamera();
        OnPlayerJump?.Invoke();
    }
    #endregion
    #region Helper Methods
    private void HandleJumpCamera()
    {
        if (playerCameraManager.cameraTiltCoroutine != null)
        {
            StopCoroutine(playerCameraManager.cameraTiltCoroutine);
        }
        playerCameraManager.cameraTiltCoroutine = StartCoroutine(playerCameraManager.TiltCameraDown());
    }

    private void LogDebugMessage(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[PlayerManager] {message}");
        }
    }
    #endregion

    #region Cleanup
    private void OnDisable()
    {
        moveAction?.Disable();
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        OnPlayerStateChanged = null;
        OnPlayerCollision = null;
        OnPlayerJump = null;
    }
    #endregion

    #region Additional Utility
    /// <summary>
    /// Defines the possible states a player can be in during gameplay
    /// </summary>
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Dodging,
        Dead
    }
    #endregion

    #region Event Management
    private void SubscribeToEvents()
    {
        OnPlayerStateChanged += HandlePlayerStateChanged;
        OnPlayerCollision += HandlePlayerCollision;
        OnPlayerJump += HandlePlayerJump;
    }

    private void UnsubscribeFromEvents()
    {
        OnPlayerStateChanged -= HandlePlayerStateChanged;
        OnPlayerCollision -= HandlePlayerCollision;
        OnPlayerJump -= HandlePlayerJump;
    }

    private void HandlePlayerStateChanged(PlayerState newState)
    {
        LogDebugMessage($"Player state changed to: {newState}");
    }

    private void HandlePlayerCollision()
    {
        LogDebugMessage("Player collision detected");
    }

    private void HandlePlayerJump()
    {
        LogDebugMessage("Player jump executed");
    }
    #endregion

    private void AssignManagerReferences()
    {
        gameManager = GameManager.Instance;
        parkourQTEManager = ParkourQTEManager.Instance;
        playerCameraManager = PlayerCameraManager.Instance;
        gameSceneUIManager = GameSceneUIManager.Instance;
    }

        /// <summary>
    /// Validates all required manager references are properly assigned
    /// </summary>
    private void ValidateManagerReferences()
    {
        if (gameManager == null)
        {
            LogDebugMessage("Game Manager is not assigned!");
        }

        if (parkourQTEManager == null)
        {
            LogDebugMessage("QTE Manager is not assigned!");
        }
        
        if (playerCameraManager == null)
        {
            LogDebugMessage("Player Camera Manager is not assigned!");
        }

        if (gameSceneUIManager == null)
        {
            LogDebugMessage("Game Scene UI Manager is not assigned!");
        }
    }

}