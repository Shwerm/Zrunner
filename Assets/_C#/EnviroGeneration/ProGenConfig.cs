using UnityEngine;

/// <summary>
/// Procedural generation config scriptable object for environment generation.
/// Controls spawn rates, distances, pooling settings and corridor management.
/// Used by ProGenManager.cs to determine environment generation behavior.
/// </summary>
[CreateAssetMenu(fileName = "ProGenConfig", menuName = "Runner/ProGen Config")]
public class ProGenConfig : ScriptableObject
{
    /// <summary>
    /// Core generation parameters for environment creation
    /// </summary>
    [Header("Generation Settings")]
    [Tooltip("Number of corridor sections to spawn at game start")]
    public int InitialCorridorCount = 4;
    
    [Tooltip("Maximum number of corridor sections allowed in scene")]
    public int MaxActiveCorridors = 5;
    
    [Tooltip("Time in seconds before unused sections are unloaded")]
    public float ChunkUnloadDelay = 4f;
    
    /// <summary>
    /// Percentage chances for different section types to spawn.
    /// Total of all chances must equal 100.
    /// </summary>
    [Header("Spawn Probabilities")]
    [Range(0, 100), Tooltip("Percentage chance of spawning a basic corridor")]
    public float CorridorSpawnChance = 60f;
    
    [Range(0, 100), Tooltip("Percentage chance of spawning an obstacle section")]
    public float ObstacleSpawnChance = 20f;
    
    [Range(0, 100), Tooltip("Percentage chance of spawning an enemy section")]
    public float EnemySpawnChance = 20f;
    
    /// <summary>
    /// Physical placement and pooling configuration.
    /// </summary>
    [Header("Spawn Settings")]
    [Tooltip("Distance between spawned sections in world units")]
    public float SpawnDistance = 5f;

    [Header("Pool Settings")]
    [Tooltip("Number of pre-instantiated sections per type in object pool")]
    public int PoolSizePerSection = 10;
}
