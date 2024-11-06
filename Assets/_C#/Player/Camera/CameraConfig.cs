using UnityEngine;

/// <summary>
/// Scriptable Object defining camera behavior and movement parameters.
/// Controls timing, angles, and animation curves for camera transitions.
/// 
/// Key Features:
/// - Configurable tilt and hold durations
/// - Customizable viewing angles
/// - Animation curve-based movement smoothing
/// - Editor-friendly parameter adjustment
/// </summary>
[CreateAssetMenu(fileName = "CameraConfig", menuName = "Camera/Camera Config")]
public class CameraConfig : ScriptableObject
{
    /// <summary>
    /// Duration settings for camera movements and holds
    /// </summary>
    [Header("Timing")]
    public float tiltDuration = 1.1f;    // Time taken to complete tilt animation
    public float holdDuration = 1.5f;    // Duration to maintain tilted position

    /// <summary>
    /// Angle configurations for different camera views
    /// </summary>
    [Header("Angles")]
    public float downTiltAngle = 90f;    // Vertical tilt angle for looking down
    public float sideTiltAngle = 90f;    // Horizontal tilt angle for side views

    /// <summary>
    /// Animation curve controlling tilt movement easing
    /// </summary>
    [Header("Smoothing")]
    public AnimationCurve tiltEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

