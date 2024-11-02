using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour, IPlayerMovement, IPlayerCollision
{
    #region Constants
    private const float JUMP_HEIGHT = 6f;
    private const float TIME_SCALE_COMBAT = 0.5f;
    private const float TIME_SCALE_PARKOUR = 0.2f;
    #endregion

    #region Events
    public event System.Action<PlayerState> OnPlayerStateChanged;
    public event System.Action<string> OnQTETriggered;
    #endregion

    #region Singleton
    public static PlayerManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("QTE Settings")]
    [SerializeField, Tooltip("Speed at which player dodges left/right")]
    private float dodgeSpeed = 10f;
    #endregion

    #region State Management
    public enum PlayerState
    {
        Running,
        Jumping,
        Dodging,
        Combat
    }
    private PlayerState currentState = PlayerState.Running;
    #endregion

    #region Public Properties
    public string activeParkourQTE { get; private set; }
    public string activeCombatQTE { get; private set; }
    #endregion

    #region Private Fields
    private GameManager gameManager;
    private ParkourQTEManager parkourQTEManager;
    private PlayerCameraManager playerCameraManager;
    private GameSceneUIManager gameSceneUIManager;
    private IInputService inputService;
    private Rigidbody rb;
    private Transform cachedTransform;
    private Vector3 targetPosition;
    private float originalXPosition;
    private Queue<Coroutine> coroutinePool = new Queue<Coroutine>();
    #endregion

    private void Awake()
    {
        InitializeSingleton();
        CacheComponents();
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
        cachedTransform = transform;
    }

    public void Initialize(
        GameManager gameManager,
        ParkourQTEManager parkourQTEManager,
        PlayerCameraManager cameraManager,
        GameSceneUIManager uiManager,
        IInputService inputService)
    {
        this.gameManager = gameManager;
        this.parkourQTEManager = parkourQTEManager;
        this.playerCameraManager = cameraManager;
        this.gameSceneUIManager = uiManager;
        this.inputService = inputService;
        
        originalXPosition = cachedTransform.position.x;
        ValidateManagerReferences();
    }

    private void ValidateManagerReferences()
    {
        if (gameManager == null)
        {
            Debug.LogError("[PlayerManager] Game Manager is not assigned!");
        }

        if (parkourQTEManager == null)
        {
            Debug.LogError("[PlayerManager] QTE Manager is not assigned!");
        }
        
        if (playerCameraManager == null)
        {
            Debug.LogError("[PlayerManager] Player Camera Manager is not assigned!");
        }

        if (gameSceneUIManager == null)
        {
            Debug.LogError("[PlayerManager] Game Scene UI Manager is not assigned!");
        }
    }

    public void Move(float deltaTime)
    {
        if (currentState != PlayerState.Running) return;
        cachedTransform.Translate(Vector3.forward * moveSpeed * deltaTime);
    }

    private void Update()
    {
        Move(Time.deltaTime);
    }

    public void HandleCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            try
            {
                gameManager.playerDeath();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlayerManager] Failed to process death: {e.Message}");
            }
        }
    }

    public void HandleTrigger(Collider other)
    {
        try
        {
            ProcessTrigger(other);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerManager] Trigger processing failed: {e.Message}");
        }
    }

    private void ProcessTrigger(Collider other)
    {
        if (other.CompareTag("Jump"))
        {
            SetQTEState("Jump", PlayerState.Jumping);
        }
        if (other.CompareTag("Slide"))
        {
            SetQTEState("Slide", PlayerState.Running);
        }
        if (other.CompareTag("DodgeRight"))
        {
            SetQTEState("DodgeRight", PlayerState.Dodging);
        }
        if (other.CompareTag("DodgeLeft"))
        {
            SetQTEState("DodgeLeft", PlayerState.Dodging);
        }
        if (other.CompareTag("Left"))
        {
            activeCombatQTE = "Left";
            Time.timeScale = TIME_SCALE_COMBAT;
            gameSceneUIManager.combatQteVisualTrigger(activeCombatQTE);
        }
        if (other.CompareTag("Right"))
        {
            activeCombatQTE = "Right";
            Time.timeScale = TIME_SCALE_COMBAT;
            gameSceneUIManager.combatQteVisualTrigger(activeCombatQTE);
        }
    }

    private void SetQTEState(string qteType, PlayerState newState)
    {
        currentState = newState;
        activeParkourQTE = qteType;
        OnPlayerStateChanged?.Invoke(newState);
        OnQTETriggered?.Invoke(qteType);
        parkourQTEManager.parkourQteStart();
    }

    public void Dodge(float dodgeLeftRightAmount)
    {
        targetPosition = cachedTransform.position;
        targetPosition.x = dodgeLeftRightAmount;
        StartCoroutine(SmoothDodge());
    }

    public void ReverseDodge()
    {
        targetPosition = cachedTransform.position;
        targetPosition.x = originalXPosition;
        StartCoroutine(SmoothDodge());
    }

    private IEnumerator SmoothDodge()
    {
        while (cachedTransform.position.x != targetPosition.x)
        {
            cachedTransform.position = new Vector3
            (
                Mathf.MoveTowards(cachedTransform.position.x, targetPosition.x, dodgeSpeed * Time.deltaTime),
                cachedTransform.position.y,
                cachedTransform.position.z
            );

            yield return null;
        }
    }

    public void Jump()
    {
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * JUMP_HEIGHT);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        if (playerCameraManager.cameraTiltCoroutine != null)
        {
            StopCoroutine(playerCameraManager.cameraTiltCoroutine);
        }
        playerCameraManager.cameraTiltCoroutine = StartCoroutine(playerCameraManager.TiltCameraDown());
    }
}