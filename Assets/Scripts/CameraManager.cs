using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera nextCam, Previous;

    // Move to next cam
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            nextCam.Priority = 2;
            Previous.Priority = 1;
        }
    }
}
