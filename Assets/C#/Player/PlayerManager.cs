using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 8f;

    [Header("References")]
    public GameManager gameManager;


    void Start()
    {
        // If the gameManager is not assigned in the inspector, try to find it in the scene
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
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
}
