using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages the player camera, including tilt and movement during QTEs.
/// Dependencies: N/A
/// </summary>
public class PlayerCameraManager : MonoBehaviour
{
    #region Singleton
    public static PlayerCameraManager Instance { get; private set; }
    #endregion

    #region Camera Settings
    [SerializeField]private Camera playerCamera;
    private float cameraTiltDuration = 1.1f;
    public Coroutine cameraTiltCoroutine;
    #endregion


    /// <summary>
    /// Initializes the singleton instance on Awake
    /// </summary>
    void Awake()
    {
        //Create Singleton Instance of the PlayerCamera
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    /// <summary>
    /// Function for tilting the camera down during Jump QTEs
    /// - Tilt the camera down to 90 degrees
    /// - Wait for the QTE to complete
    /// - Reset the camera rotation
    /// </summary>
    public IEnumerator TiltCameraDown()
    {
        //Store the original rotation
        Quaternion originalRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(90f, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);

        //Tilt up to 90 degrees
        float elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = targetRotation;

        //Tilt back to original rotation
        elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = originalRotation;
    }


    /// <summary>
    ///Function for tilting the camera right during Look Right QTEs
    ///- Tilt the camera to 90 degrees on the Y-axis
    ///- Wait for the QTE to complete
    ///- Reset the camera rotation
    /// </summary>
    public void LookRight()
    {
        //Start the camera tilt coroutine
        if (cameraTiltCoroutine != null)
        {
            StopCoroutine(cameraTiltCoroutine); //Stop any existing tilt before starting a new one
        }
        cameraTiltCoroutine = StartCoroutine(TiltCameraRight());
    }

    private IEnumerator TiltCameraRight()
    {
        //Store the original rotation
        Quaternion originalRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(playerCamera.transform.eulerAngles.x, 90f, playerCamera.transform.eulerAngles.z);

        //Tilt to 90 degrees on the Y-axis
        float elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = targetRotation; //Ensure it ends at exactly 90 degrees

        //Tilt back to the original rotation
        elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = originalRotation; //Ensure it ends at original rotation
    }


    /// <summary>
    ///Function for tilting the camera left during Look Left QTEs
    ///- Tilt the camera to -90 degrees on the Y-axis
    ///- Wait for the QTE to complete
    ///- Reset the camera rotation
    /// </summary>
    public void LookLeft()
    {
        //Start the camera tilt coroutine
        if (cameraTiltCoroutine != null)
        {
            StopCoroutine(cameraTiltCoroutine); //Stop any existing tilt before starting a new one
        }
        cameraTiltCoroutine = StartCoroutine(TiltCameraLeft());
    }

    private IEnumerator TiltCameraLeft()
    {
        //Store the original rotation
        Quaternion originalRotation = playerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(playerCamera.transform.eulerAngles.x, -90f, playerCamera.transform.eulerAngles.z);

        //Tilt to 90 degrees on the Y-axis
        float elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = targetRotation; //Ensure it ends at exactly 90 degrees

        //Tilt back to the original rotation
        elapsedTime = 0f;
        while (elapsedTime < cameraTiltDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCamera.transform.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / cameraTiltDuration);
            yield return null;
        }
        playerCamera.transform.rotation = originalRotation; //Ensure it ends at original rotation
    }
}
