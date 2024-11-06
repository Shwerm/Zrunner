using UnityEngine;
using System.Collections;

/// <summary>
/// Core movement system handling player locomotion and dynamic actions.
/// Controls forward movement, dodging mechanics, and jump sequences.
/// 
/// Key Features:
/// - Continuous forward movement
/// - Smooth dodge transitions
/// - Physics-based jumping
/// - Difficulty-based speed scaling
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Singleton
    public static PlayerMovement Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField, Tooltip("Speed at which player dodges left/right")] 
    private float dodgeSpeed = 10f;
    #endregion

    #region Private Fields
    private Vector3 targetPosition;
    private float originalXPosition;
    private PlayerCameraManager playerCameraManager;
    #endregion

    #region Initialization
    private void Start()
    {
        originalXPosition = transform.position.x;
        playerCameraManager = PlayerCameraManager.Instance;
    }
    #endregion

    #region Movement
    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Executes a lateral dodge movement to specified position.
    /// Smoothly transitions player to target X coordinate.
    /// </summary>
    /// <param name="dodgeLeftRightAmount">Target X position for dodge</param>
    public void Dodge(float dodgeLeftRightAmount)
    {
        targetPosition = transform.position;
        targetPosition.x = dodgeLeftRightAmount;
        StartCoroutine(SmoothDodge());
    }

    /// <summary>
    /// Returns player to original lateral position.
    /// Used after section completion or dodge recovery.
    /// </summary>
    public void ReverseDodge()
    {
        targetPosition = transform.position;
        targetPosition.x = originalXPosition;
        StartCoroutine(SmoothDodge());
    }

    /// <summary>
    /// Executes smooth interpolation for dodge movements.
    /// Handles position updates and movement completion.
    /// </summary>
    private IEnumerator SmoothDodge()
    {
        while (transform.position.x != targetPosition.x)
        {
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, targetPosition.x, dodgeSpeed * Time.deltaTime),
                transform.position.y,
                transform.position.z
            );
            yield return null;
        }
    }

    /// <summary>
    /// Performs physics-based jump with camera feedback.
    /// Calculates jump force and initiates camera tilt sequence.
    /// </summary>
    public void Jump()
    {
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 6f);
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (playerCameraManager.cameraTiltCoroutine != null)
        {
            StopCoroutine(playerCameraManager.cameraTiltCoroutine);
        }
        playerCameraManager.cameraTiltCoroutine = StartCoroutine(playerCameraManager.TiltCameraDown());
    }

    /// <summary>
    /// Updates movement speeds based on current difficulty level.
    /// Scales both forward and dodge speeds according to time manager multipliers.
    /// </summary>
    /// <param name="difficultyLevel">Current game difficulty level</param>
    public void UpdatePlayerSpeed(int difficultyLevel)
    {
        moveSpeed *= TimeManager.Instance.CurrentSpeedMultiplier;
        dodgeSpeed = 10f * TimeManager.Instance.GetDodgeSpeedMultiplier();
    }
    #endregion
}
