using UnityEngine;

[CreateAssetMenu(fileName = "ProGenConfig", menuName = "Procedural Generation/ProGen Config")]
public class ProGenConfig : ScriptableObject
{
    [Header("Generation Settings")]
    public int InitialCorridorCount = 4;
    public int MaxActiveCorridors = 5;
    public float ChunkUnloadDelay = 4f;
    
    [Header("Spawn Probabilities")]
    [Range(0, 100)] public float CorridorSpawnChance = 60f;
    [Range(0, 100)] public float ObstacleSpawnChance = 20f;
    [Range(0, 100)] public float EnemySpawnChance = 20f;
    
    [Header("Spawn Settings")]
    public float SpawnDistance = 5f;

    [Header("Pool Settings")]
    public int PoolSizePerSection = 10;
}
