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

    public string activeQTE;


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
        //If the Game Manager is not assigned in the inspector, try to find it in the scene
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        //If the QTE Manager is not assigned in the inspector, try to find it in the scene
        if (qteManager == null)
        {
            qteManager = FindObjectOfType<QTEManager>();
        }
    }


    void Update()
    {
        //Move the player
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        Debug.Log(Time.timeScale);
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
}
