using UnityEngine;

/// <summary>
/// Scriptable Object defining camera behavior and movement parameters.
/// Controls timing, angles, and tilt curves for camera transitions.
/// </summary>
[CreateAssetMenu(fileName = "CameraConfig", menuName = "Camera/Camera Config")]
public class CameraConfig : ScriptableObject
{
    /// <summary>
    /// Duration settings for camera movements and holds
    /// </summary>
    [Header("Timing")]
    public float tiltDuration = 1.1f;    
    public float holdDuration = 1.5f;    

    /// <summary>
    /// Angle configurations for different camera views
    /// </summary>
    [Header("Angles")]
    public float downTiltAngle = 90f;    
    public float sideTiltAngle = 90f;    

    /// <summary>
    /// Animation curve controlling tilt movement easing
    /// </summary>
    [Header("Smoothing")]
    public AnimationCurve tiltEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
}

