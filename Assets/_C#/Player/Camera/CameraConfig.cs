using UnityEngine;

/// <summary>
/// Configuration settings for camera movement and behavior
/// </summary>
[CreateAssetMenu(fileName = "CameraConfig", menuName = "Camera/Camera Config")]
public class CameraConfig : ScriptableObject
{
    [Header("Timing")]
    public float tiltDuration = 1.1f;
    public float holdDuration = 1.5f;

    [Header("Angles")]
    public float downTiltAngle = 90f;
    public float sideTiltAngle = 90f;

    [Header("Smoothing")]
    public AnimationCurve tiltEasing = AnimationCurve.EaseInOut(0, 0, 1, 1);
}
