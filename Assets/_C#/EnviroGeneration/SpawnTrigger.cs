using System;
using UnityEngine;

/// <summary>
/// Handles the triggering of new corridor/obstacle section spawns and player position resets
/// Attached to trigger volumes at the end of each section
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
    /// Locates and validates required game objects and components
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

    public void HandleTriggerEnter(Collider other)
    {
        OnTriggerEnter(other);
    }

    /// <summary>
    /// Generates next section and resets player position
    /// </summary>
    private void HandleSpawn()
    {
        if (!enabled) return;

        ProGenManager.Instance.SpawnCorridor();
        PlayerManager.Instance.ReverseDodge();
    }
    #endregion

    #region Debug Visualization
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