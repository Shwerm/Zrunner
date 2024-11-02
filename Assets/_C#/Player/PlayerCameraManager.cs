using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the player camera, including tilt and movement during QTEs.
/// Implements smooth camera transitions and state management.
/// Dependencies: CameraConfig
/// </summary>
public class PlayerCameraManager : MonoBehaviour, ICameraController
{
    #region Singleton
    public static PlayerCameraManager Instance { get; private set; }
    #endregion

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
    /// Initiates a downward camera tilt for jump sequences
    /// </summary>
    public IEnumerator TiltCameraDown()
    {
        UpdateCameraState(CameraState.Tilting);
        Vector3 targetAngles = new Vector3(90f, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);
        yield return TiltCamera(targetAngles, cameraConfig.holdDuration);
        UpdateCameraState(CameraState.Normal);
    }

    /// <summary>
    /// Initiates a right-facing camera tilt
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
    /// Initiates a left-facing camera tilt
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
    private IEnumerator TiltCamera(Vector3 targetEulerAngles, float holdDuration)
    {
        UpdateCameraState(CameraState.Tilting);
        Quaternion originalRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        yield return PerformTilt(originalRotation, targetRotation);
        
        UpdateCameraState(CameraState.Held);
        yield return new WaitForSeconds(holdDuration);
        
        yield return PerformTilt(targetRotation, originalRotation);
        UpdateCameraState(CameraState.Normal);
    }

    private IEnumerator PerformTilt(Quaternion from, Quaternion to)
    {
        float elapsedTime = 0f;
        while (elapsedTime < cameraConfig.tiltDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / cameraConfig.tiltDuration;
            
            playerCamera.transform.rotation = Quaternion.Slerp(from, to, progress);
            OnTiltProgress?.Invoke(progress);
            
            yield return null;
        }
        playerCamera.transform.rotation = to;
    }

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
