using UnityEngine;

/// <summary>
/// Configurable settings for player behavior and movement
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float dodgeSpeed = 10f;
    public float jumpForce = 6f;

    [Header("QTE Settings")]
    public float slowMotionTimeScale = 0.5f;
    public float normalTimeScale = 1f;
}
