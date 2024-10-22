using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 8f;

    [Header("References")]
    public GameManager gameManager;
    public QTEManager qteManager;


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
            qteManager.JumpQTE();
        }

        if (other.CompareTag("Slide"))
        {
            qteManager.SlideQTE();
        }

        if (other.CompareTag("DodgeRight"))
        {
            qteManager.DodgeRightQTE();
        }

        if (other.CompareTag("DodgeLeft"))
        {
            qteManager.DodgeLeftQTE();
        }
    }
}
