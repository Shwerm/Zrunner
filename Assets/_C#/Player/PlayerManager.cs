using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages player core functionality including movement, collision handling, and QTE interactions.
/// Handles forward movement, dodge mechanics, jumping, and obstacle detection.
/// Dependencies: GameManager.cs, QTEManager.cs, PlayerCameraManager.cs
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Player Settings")]
    [SerializeField]public float moveSpeed = 8f;

    [Header("QTE Settings")]
    [SerializeField, Tooltip("Speed at which player dodges left/right")]
    private float dodgeSpeed = 10f;
    #endregion

    public string activeParkourQTE;
    public string activeCombatQTE;

    #region Private Fields
    private GameManager gameManager;
    private ParkourQTEManager parkourQTEManager;
    private PlayerCameraManager playerCameraManager;
    private GameSceneUIManager gameSceneUIManager;

    private Vector3 targetPosition;
    private float originalXPosition;
    #endregion


    /// <summary>
    /// Initializes the singleton instance of PlayerManager
    /// Ensures only one instance exists in the game
    /// </summary>
    private void Awake()
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


    /// <summary>
    /// Initializes player components and validates required dependencies
    /// </summary>
    void Start()
    {
        //Store the initial X position of the player
        originalXPosition = transform.position.x;

        //Assign references
        gameManager = GameManager.Instance;
        parkourQTEManager = ParkourQTEManager.Instance;
        playerCameraManager = PlayerCameraManager.Instance;
        gameSceneUIManager = GameSceneUIManager.Instance;

        ValidateManagerReferences();
    }


    /// <summary>
    /// Validates all required manager references are properly assigned
    /// </summary>
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
    

    /// <summary>
    /// Handles continuous forward movement of the player
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    
    /// <summary>
    /// Processes collision events with game obstacles
    /// Triggers player death state on valid collision
    /// </summary>
    /// <param name="collision">Collision data from the physics system</param>
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.playerDeath();
        }
    }


    /// <summary>
    /// Detects and processes QTE trigger zones in the game world
    /// Initiates appropriate QTE sequences based on trigger type (Jump, Slide, Dodge)
    /// </summary>
    /// <param name="other">Collider that entered the trigger zone</param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jump"))
        {
            activeParkourQTE = "Jump";
            parkourQTEManager.parkourQteStart();
        }

        if (other.CompareTag("Slide"))
        {
            activeParkourQTE = "Slide";
            parkourQTEManager.parkourQteStart();
        }

        if (other.CompareTag("DodgeRight"))
        {
            activeParkourQTE = "DodgeRight";
            parkourQTEManager.parkourQteStart();
        }

        if (other.CompareTag("DodgeLeft"))
        {
            activeParkourQTE = "DodgeLeft";
            parkourQTEManager.parkourQteStart();
        }


        if (other.CompareTag("Left"))
        {
            activeCombatQTE = "Left";
            Time.timeScale = 0.5f;
            gameSceneUIManager.combatQteVisualTrigger(activeCombatQTE);
        }

        if (other.CompareTag("Right"))
        {
            activeCombatQTE = "Right";
            Time.timeScale = 0.5f;
            gameSceneUIManager.combatQteVisualTrigger(activeCombatQTE);
        }
    }
 
    
    /// <summary>
    /// Initiates a dodge movement to specified X position
    /// </summary>
    /// <param name="dodgeLeftRightAmount">Target X position for dodge</param>
    public void Dodge(float dodgeLeftRightAmount)
    {
        targetPosition = transform.position;
        targetPosition.x = dodgeLeftRightAmount;
        StartCoroutine(SmoothDodge());
    }


    /// <summary>
    /// Returns player to original X position after dodge
    /// </summary>
    public void ReverseDodge()
    {
        targetPosition = transform.position;
        targetPosition.x = originalXPosition;
        StartCoroutine(SmoothDodge());
    }


    /// <summary>
    /// Handles smooth interpolation for dodge movements
    /// Maintains forward movement while adjusting X position
    /// </summary>
    private IEnumerator SmoothDodge()
    {
        while (transform.position.x != targetPosition.x)
        {
            transform.position = new Vector3
            (
                Mathf.MoveTowards(transform.position.x, targetPosition.x, dodgeSpeed * Time.deltaTime),
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }
    }


    /// <summary>
    /// Executes player jump with physics-based force
    /// Coordinates camera tilt effect during jump
    /// </summary>
    public void Jump()
    {
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 6f);
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (playerCameraManager.cameraTiltCoroutine != null)
        {
            StopCoroutine(playerCameraManager.cameraTiltCoroutine);
        }
        playerCameraManager.cameraTiltCoroutine = StartCoroutine(playerCameraManager.TiltCameraDown());
    }

    private void OnEnable()
    {
        // Wait for next frame to ensure TimeManager is initialized
        StartCoroutine(SubscribeToTimeManager());
    }

    private IEnumerator SubscribeToTimeManager()
    {
        yield return new WaitForEndOfFrame();
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDifficultyChanged += UpdatePlayerSpeed;
        }
    }
    
    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDifficultyChanged -= UpdatePlayerSpeed;
    }

    private void UpdatePlayerSpeed(int difficultyLevel)
    {
        moveSpeed *= TimeManager.Instance.CurrentSpeedMultiplier;
    }
}