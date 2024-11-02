using UnityEngine;

[CreateAssetMenu(fileName = "ProGenConfig", menuName = "Procedural Generation/ProGen Config")]
public class ProGenConfig : ScriptableObject
{
    [Header("Generation Settings")]
    public int InitialCorridorCount = 4;
    public int MaxActiveCorridors = 5;
    public float ChunkUnloadDelay = 4f;
    
    [Header("Spawn Settings")]
    [Range(1, 13)] public int ObstacleSpawnChanceThreshold = 4;
    [Range(1, 13)] public int EnemySpawnChanceThreshold = 11;
    public float SpawnDistance = 5f;

    [Header("Pool Settings")]
    public int PoolSizePerSection = 10;
}
