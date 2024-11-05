using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all player movement mechanics including forward movement, dodging, and jumping
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

    public void Dodge(float dodgeLeftRightAmount)
    {
        targetPosition = transform.position;
        targetPosition.x = dodgeLeftRightAmount;
        StartCoroutine(SmoothDodge());
    }

    public void ReverseDodge()
    {
        targetPosition = transform.position;
        targetPosition.x = originalXPosition;
        StartCoroutine(SmoothDodge());
    }

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

    public void UpdatePlayerSpeed(int difficultyLevel)
    {
        moveSpeed *= TimeManager.Instance.CurrentSpeedMultiplier;
        dodgeSpeed = 10f * TimeManager.Instance.GetDodgeSpeedMultiplier();
    }
    #endregion
}
