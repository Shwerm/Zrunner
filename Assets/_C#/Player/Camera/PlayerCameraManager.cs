using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Core camera management system handling player perspective and QTE transitions.
/// Provides smooth camera movements, state management, and event-driven updates.
/// 
/// Key Features:
/// - Configurable camera transitions
/// - State-driven behavior system
/// - Event-based progress tracking
/// - Singleton architecture for global access
/// </summary>
public class PlayerCameraManager : MonoBehaviour, ICameraController
{
    #region Singleton
    public static PlayerCameraManager Instance { get; private set; }
    #endregion

    /// <summary>
    /// Notifies listeners of camera state transitions between Normal, Tilting, and Held states
    /// Broadcasts tilt animation progress for UI synchronization
    /// </summary>
    #region Events
    public event Action<CameraState> OnCameraStateChanged;
    public event Action<float> OnTiltProgress;
    #endregion

    #region Serialized Fields
    [Header("Configuration")]
    [SerializeField] private CameraConfig cameraConfig;
    [SerializeField] private Camera playerCamera;
    #endregion

    #region Public Fields
    public Coroutine cameraTiltCoroutine;
    #endregion

    #region Private Fields
    private CameraState currentState = CameraState.Normal;
    #endregion

    #region Lifecycle Methods
    /// <summary>
    /// Initializes the singleton instance and validates components
    /// </summary>
    void Awake()
    {
        InitializeSingleton();
        ValidateComponents();
    }
    #endregion

    #region Initialization
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void ValidateComponents()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("[PlayerCameraManager] No camera assigned or found in children");
            }
        }

        if (cameraConfig == null)
        {
            Debug.LogError("[PlayerCameraManager] Camera config not assigned");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Executes a downward camera tilt sequence for jump animations.
    /// Manages state transitions and timing based on configuration.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution</returns>
    public IEnumerator TiltCameraDown()
    {
        UpdateCameraState(CameraState.Tilting);
        Vector3 targetAngles = new Vector3(90f, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);
        yield return TiltCamera(targetAngles, cameraConfig.holdDuration);
        UpdateCameraState(CameraState.Normal);
    }

    /// <summary>
    /// Initiates a right-facing camera transition.
    /// Handles coroutine management and rotation calculations.
    /// </summary>
    public void LookRight()
    {
        SafeStopCoroutine();
        Vector3 targetAngles = new Vector3(
            playerCamera.transform.eulerAngles.x,
            90f,
            playerCamera.transform.eulerAngles.z
        );
        cameraTiltCoroutine = StartCoroutine(TiltCamera(targetAngles, cameraConfig.holdDuration));
    }

    /// <summary>
    /// Initiates a left-facing camera transition.
    /// Handles coroutine management and rotation calculations.
    /// </summary>
    public void LookLeft()
    {
        SafeStopCoroutine();
        Vector3 targetAngles = new Vector3(
            playerCamera.transform.eulerAngles.x,
            -90f,
            playerCamera.transform.eulerAngles.z
        );
        cameraTiltCoroutine = StartCoroutine(TiltCamera(targetAngles, cameraConfig.holdDuration));
    }
    #endregion

    #region Private Methods
    private float GetAdjustedTiltDuration()
    {
        return cameraConfig.tiltDuration * TimeManager.Instance.GetCameraTiltMultiplier();
    }

    private float GetAdjustedHoldDuration()
    {
        return cameraConfig.holdDuration * TimeManager.Instance.GetCameraHoldMultiplier();
    }

    /// <summary>
    /// Executes camera tilt animation with timing and state management.
    /// </summary>
    /// <param name="targetEulerAngles">Target rotation angles</param>
    /// <param name="holdDuration">Duration to maintain end position</param>
    private IEnumerator TiltCamera(Vector3 targetEulerAngles, float holdDuration)
    {
        UpdateCameraState(CameraState.Tilting);
        Quaternion originalRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        yield return PerformTilt(originalRotation, targetRotation);
        
        UpdateCameraState(CameraState.Held);
        yield return new WaitForSeconds(GetAdjustedHoldDuration());
        
        yield return PerformTilt(targetRotation, originalRotation);
        UpdateCameraState(CameraState.Normal);
    }

    /// <summary>
    /// Performs smooth camera rotation between two orientations.
    /// </summary>
    /// <param name="from">Starting rotation</param>
    /// <param name="to">Target rotation</param>
    private IEnumerator PerformTilt(Quaternion from, Quaternion to)
    {
        float elapsedTime = 0f;
        float adjustedTiltDuration = GetAdjustedTiltDuration();
        
        while (elapsedTime < adjustedTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / adjustedTiltDuration;
            
            playerCamera.transform.rotation = Quaternion.Slerp(from, to, progress);
            OnTiltProgress?.Invoke(progress);
            
            yield return null;
        }
        playerCamera.transform.rotation = to;
    }

    /// <summary>
    /// Safely terminates active camera transitions.
    /// Handles edge cases and logs errors for debugging.
    /// </summary>
    private void SafeStopCoroutine()
    {
        try
        {
            if (cameraTiltCoroutine != null)
            {
                StopCoroutine(cameraTiltCoroutine);
                cameraTiltCoroutine = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerCameraManager] Failed to stop camera coroutine: {e.Message}");
        }
    }

    /// <summary>
    /// Updates camera state and notifies listeners of state changes.
    /// </summary>
    /// <param name="newState">Target camera state</param>
    private void UpdateCameraState(CameraState newState)
    {
        currentState = newState;
        OnCameraStateChanged?.Invoke(currentState);
    }
    #endregion
}

#region Supporting Types
public enum CameraState
{
    Normal,
    Tilting,
    Held
}

public interface ICameraController
{
    void LookRight();
    void LookLeft();
    IEnumerator TiltCameraDown();
}
#endregion
