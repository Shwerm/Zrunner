using UnityEngine;

/// <summary>
/// Manages player collision detection and response system.
/// Processes parkour and combat trigger interactions for QTE initiation.
/// 
/// Key Features:
/// - Collision-based death handling
/// - QTE trigger detection
/// - Combat encounter initialization
/// - Time scale manipulation
/// </summary>
public class PlayerCollisionHandler : MonoBehaviour
{
    #region Private Fields
    private PlayerManager playerManager;
    private GameManager gameManager;
    private ParkourQTEManager parkourQTEManager;
    private GameSceneUIManager gameSceneUIManager;
    #endregion

    #region Initialization
    private void Start()
    {
        playerManager = PlayerManager.Instance;
        gameManager = GameManager.Instance;
        parkourQTEManager = ParkourQTEManager.Instance;
        gameSceneUIManager = GameSceneUIManager.Instance;
    }
    #endregion

    #region Collision Handling
    /// <summary>
    /// Processes collision events with obstacles and environmental hazards.
    /// Triggers player death state on obstacle impacts.
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
    /// Handles trigger volume interactions for both parkour and combat events.
    /// Routes triggers to appropriate subsystems for processing.
    /// </summary>
    /// <param name="other">Trigger collider that initiated the interaction</param>
    public void OnTriggerEnter(Collider other)
    {
        HandleParkourTriggers(other);
        HandleCombatTriggers(other);
    }

    /// <summary>
    /// Processes parkour-specific trigger interactions.
    /// Initiates appropriate QTE sequences based on trigger type.
    /// </summary>
    /// <param name="other">Trigger collider containing parkour action type</param>
    private void HandleParkourTriggers(Collider other)
    {
        if (other.CompareTag("Jump") || other.CompareTag("Slide") || 
            other.CompareTag("DodgeRight") || other.CompareTag("DodgeLeft"))
        {
            playerManager.activeParkourQTE = other.tag;
            parkourQTEManager.parkourQteStart();
        }
    }

    /// <summary>
    /// Manages combat encounter trigger interactions.
    /// Initiates combat QTEs and adjusts time scale for combat sequences.
    /// </summary>
    /// <param name="other">Trigger collider containing combat direction</param>
    private void HandleCombatTriggers(Collider other)
    {
        if (other.CompareTag("Left") || other.CompareTag("Right"))
        {
            playerManager.activeCombatQTE = other.tag;
            Time.timeScale = 0.5f;
            gameSceneUIManager.combatQteVisualTrigger(playerManager.activeCombatQTE);
        }
    }
    #endregion
}
