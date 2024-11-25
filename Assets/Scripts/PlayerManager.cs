using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;  
    [SerializeField] private float rotationSpeed = 100f;  
    [SerializeField] private float jumpForce = 4f;  
    [SerializeField] private bool isGrounded;

    private Rigidbody rb;
    public float interactionRange = 10000f; 
    [SerializeField] private Camera playerPOVCamera;

    private CameraButton cameraButton;

    public bool LookingAtObject1 = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraButton = FindObjectOfType<CameraButton>();
    }

    void FindPlayerPOVCamera()
    {
        playerPOVCamera = GameObject.Find("PlayerPOVCamera")?.GetComponent<Camera>();

        // Handle the case when the camera is still not found
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalInput * moveSpeed;
        float rotationInput = horizontalInput * rotationSpeed;

        rb.MovePosition(rb.position + moveDirection * Time.deltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rotationInput * Time.deltaTime));

        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (playerPOVCamera == null)
        {
            // If the camera is still null, try to find it again
            FindPlayerPOVCamera();
        }

        if (playerPOVCamera != null && cameraButton != null && cameraButton.CameraTaskActive == true)
        {
            // Debug information for troubleshooting
            Debug.DrawRay(playerPOVCamera.transform.position, playerPOVCamera.transform.forward, Color.red, 0.1f);

            if (Physics.Raycast(playerPOVCamera.transform.position, playerPOVCamera.transform.forward, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Check if the object has the "Mirror" tag
                if (hitObject.CompareTag("Mirror"))
                {
                    Debug.Log("Player is looking at a mirror!");
                    LookingAtObject1 = true;
                }
                else
                {
                    LookingAtObject1 = false;
                }
            }
        }
    }
}
