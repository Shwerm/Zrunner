using UnityEngine;

/// <summary>
/// Handles all collision and trigger interactions for the player
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
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.playerDeath();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        HandleParkourTriggers(other);
        HandleCombatTriggers(other);
    }

    private void HandleParkourTriggers(Collider other)
    {
        if (other.CompareTag("Jump") || other.CompareTag("Slide") || 
            other.CompareTag("DodgeRight") || other.CompareTag("DodgeLeft"))
        {
            playerManager.activeParkourQTE = other.tag;
            parkourQTEManager.parkourQteStart();
        }
    }

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
