using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCameraTask : MonoBehaviour
{
    private CameraButton cameraButton;
    private PlayerManager playerManager;

    public GameObject blackViewFinder;

    // Start is called before the first frame update
    void Start()
    {
        cameraButton = FindObjectOfType<CameraButton>();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cameraButton != null && cameraButton.CameraTaskActive == true && playerManager!= null && playerManager.LookingAtObject1 == true)
        {
            blackViewFinder.SetActive(false);
        }
        else if(cameraButton != null && cameraButton.CameraTaskActive == true && playerManager!= null && playerManager.LookingAtObject1 == false)
        {
            blackViewFinder.SetActive(true);
        }
        
    }
}
