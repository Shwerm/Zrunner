using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Define Singleton instance
    public static PlayerManager playerManagerInstance { get; private set; }

    [Header("Player Settings")]
    public float moveSpeed = 8f;

    [Header("References")]
    public GameManager gameManager;
    public QTEManager qteManager;

    [Header("QTE Settings")]
    public string activeQTE;
    public float dodgeSpeed = 10f;
    private Vector3 targetPosition;
    private float originalXPosition;


    //Create Singleton Instance of the Player Manager
    private void Awake()
    {
        if (playerManagerInstance == null)
        {
            playerManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        //If the Game Manager is not assigned in the inspector manually find it
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        else
        {
            Debug.LogError("Game Manager is not assigned!");
        }

        //If the QTE Manager is not assigned in the inspector manually find it
        if (qteManager == null)
        {
            qteManager = FindObjectOfType<QTEManager>();
        }
        else
        {
            Debug.LogError("QTE Manager is not assigned!");
        }

        //Store the initial X position of the player
        originalXPosition = transform.position.x;
    }


    void Update()
    {
        //Move the player
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }


    public void OnCollisionEnter(Collision collision)
    {
        //Check if the player collides with an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.playerDeath();
        }
    }


    //QTE trigger detection
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jump"))
        {
            activeQTE = "Jump";
            qteManager.qteStart();
        }

        if (other.CompareTag("Slide"))
        {
            activeQTE = "Slide";
            qteManager.qteStart();
        }

        if (other.CompareTag("DodgeRight"))
        {
            activeQTE = "DodgeRight";
            qteManager.qteStart();
        }

        if (other.CompareTag("DodgeLeft"))
        {
            activeQTE = "DodgeLeft";
            qteManager.qteStart();
        }
    }


    //Dodging Left and Right with Smooth Movement Mechanic
    public void Dodge(float dodgeLeftRightAmount)
    {
        //Set the target X position directly to dodgeLeftRightAmount
        targetPosition = transform.position;
        targetPosition.x = dodgeLeftRightAmount;

        //Start moving the player toward the target position
        StartCoroutine(SmoothDodge());
    }

    public void ReverseDodge()
    {
        //Move back to the original X position
        targetPosition = transform.position;
        targetPosition.x = originalXPosition;

        //Start moving the player back to the original position
        StartCoroutine(SmoothDodge());
    }

    private IEnumerator SmoothDodge()
    {
        //While the player has not yet reached the target X position
        while (transform.position.x != targetPosition.x)
        {
            //Move the player toward the target X position smoothly without affecting the forward movement
            transform.position = new Vector3
            (
                Mathf.MoveTowards(transform.position.x, targetPosition.x, dodgeSpeed * Time.deltaTime),
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }
    }


    //Player Jumping Mechanic
    public void Jump()
    {
        //Calculate the jump force based on the player's current velocity
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 6f);

        //Apply the jump force to the player's rigidbody
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    //Player Sliding Mechanic
    public void Slide()
    {
        //Calculate the slide force based on the player's current velocity
        float slideForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 6f);
    }
}
