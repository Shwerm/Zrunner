using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 8f;


    void Update()
    {
        //Move the player
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
