using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Manages the procedural spawning of corridor segments based on player position.
/// Coordinates with ProGenManager to handle environment generation.
/// </summary>
[DefaultExecutionOrder(1)]
public class SpawnTrigger : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject m_Player;
    
    private PlayerManager m_PlayerManager;
    private ProGenManager m_ProGenManager;
    private bool m_IsInitialized;

    private const string k_PlayerTag = "Player";

    private void Awake()
    {
        InitializeReferences();
    }

    private void OnEnable()
    {
        if (!m_IsInitialized)
        {
            StartCoroutine(WaitForProGenManager());
        }
    }

    private void InitializeReferences()
    {
        try 
        {
            m_PlayerManager = PlayerManager.playerManagerInstance;
            m_ProGenManager = ProGenManager.Instance;
            m_Player = GameObject.FindGameObjectWithTag(k_PlayerTag);
            ValidateReferences();
            m_IsInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SpawnTrigger] Initialization failed: {e.Message}");
            enabled = false;
        }
    }

    private IEnumerator WaitForProGenManager()
    {
        var waitTime = new WaitForSeconds(0.1f);
        while (m_ProGenManager == null)
        {
            m_ProGenManager = ProGenManager.Instance;
            yield return waitTime;
        }
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (m_Player == null)
            throw new NullReferenceException("[SpawnTrigger] Player reference not found");
            
        if (m_PlayerManager == null)
            throw new NullReferenceException("[SpawnTrigger] PlayerManager instance not found");
            
        if (m_ProGenManager == null)
            throw new NullReferenceException("[SpawnTrigger] ProGenManager instance not found");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_IsInitialized || !other.CompareTag(k_PlayerTag)) return;
        
        SpawnNextCorridor();
    }

    private void SpawnNextCorridor()
    {
        try 
        {
            m_ProGenManager.SpawnCorridor();
            m_PlayerManager.ReverseDodge();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SpawnTrigger] Failed to spawn corridor: {e.Message}");
        }
    }

    public void Dispose()
    {
        m_IsInitialized = false;
        m_ProGenManager = null;
        m_PlayerManager = null;
    }

    private void OnDisable()
    {
        Dispose();
    }
}