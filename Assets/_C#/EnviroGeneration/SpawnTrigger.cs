using System;
using UnityEngine;

/// <summary>
/// Manages procedural level generation triggers and player position resets.
/// Detects player collisions to spawn new sections and maintain infinite level generation.
/// 
/// Key Features:
/// - Trigger-based section spawning
/// - Player position management
/// - Event-driven architecture
/// - Debug visualization support
/// Dependencies: PlayerManager.cs, ProGenManager.cs
/// </summary>
public interface ISpawnTrigger
{
    event Action<Collider> OnTriggerActivated;
    void HandleTriggerEnter(Collider other);
}

public class SpawnTrigger : MonoBehaviour, ISpawnTrigger
{
    #region Constants
    private const string PLAYER_TAG = "Player";
    private const string ERROR_PLAYER_NOT_FOUND = "[SpawnTrigger] Player not found!";
    #endregion

    #region Private Fields
    private GameObject player;
    #endregion

    #region Events
    /// <summary>
    /// Triggered when a valid collision occurs with the spawn trigger volume.
    /// Provides the colliding object for validation and processing.
    /// </summary>
    public event Action<Collider> OnTriggerActivated;
    #endregion

    #region Lifecycle Methods
    /// <summary>
    /// Initialize references and perform error checking on startup
    /// </summary>
    void Start()
    {
        InitializeComponents();
    }

    private void OnDisable()
    {
        OnTriggerActivated = null;
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes required components and validates critical references.
    /// Disables the trigger if dependencies are missing.
    /// </summary>
    private void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        if (player == null)
        {
            Debug.LogError(ERROR_PLAYER_NOT_FOUND);
            enabled = false;
        }
    }
    #endregion

    #region Trigger Handling
    /// <summary>
    /// Detects when player enters the trigger volume
    /// Spawns next section and resets player position
    /// </summary>
    /// <param name="other">The collider that entered the trigger volume</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG)) return;
        
        OnTriggerActivated?.Invoke(other);
        HandleSpawn();
    }

    /// <summary>
    /// Validates and processes trigger collisions.
    /// Ensures only player-triggered spawns are processed.
    /// </summary>
    /// <param name="other">The collider that entered the trigger volume</param>
    public void HandleTriggerEnter(Collider other)
    {
        OnTriggerEnter(other);
    }

    /// <summary>
    /// Processes spawn events and manages player positioning.
    /// Triggers new section generation and resets player position.
    /// </summary>
    private void HandleSpawn()
    {
        if (!enabled) return;

        ProGenManager.Instance.SpawnCorridor();
        PlayerManager.Instance.Movement.ReverseDodge();
    }
    #endregion

    #region Debug Visualization
    /// <summary>
    /// Provides visual debugging in the Unity Editor.
    /// Draws trigger bounds for easier level design.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(transform.position, collider.bounds.size);
        }
    }
    #endregion
}