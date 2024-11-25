using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlash : MonoBehaviour
{
    [Header("Image From Canvas")]
    public Image image;

    [Header("Maximim & Minimum Alpha Values")]
    public float maxA = 1f;
    public float minA = 0f;
    float startA = 1f;

    [Header("Bool Value if Flash is On")]
    public bool flashOn = false;

    [Header("Audio Sources & Clips")]
    public AudioClip shutterFiring1;
    public AudioClip shutterFiring2;
    public AudioClip shutterFiring3;
    public GameObject playerSource;

    private CameraButton cameraButton; // Reference to the CameraButton script
    private PlayerManager playerManager; //" " "

    // Start is called before the first frame update
    void Start()
    {   
        // Access Alpha Value in Color Component of Image
        image = GetComponent<Image>();
        var alphaValue = image.color;
        alphaValue.a = 0f;
        image.color = alphaValue;

        cameraButton = FindObjectOfType<CameraButton>();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !flashOn && cameraButton != null && cameraButton.CameraTaskActive && playerManager!= null && playerManager.LookingAtObject1 == true)
        {
            // Set Image of White Screens Alpha to 1
            var alphaValue = image.color;
            alphaValue.a = 1f;
            image.color = alphaValue;
            flashOn = true;

            //Random Number Generator Determines Which Shutter Firing Noise Will Play
            int randomShutterNoise;
            randomShutterNoise = Random.Range(0, 3); 
            //Debug.Log(randomShutterNoise);

            //Play OneShot of Shutter Firing Clip of Corresponding Random Number
            if(randomShutterNoise == 0)
            {
                playerSource.GetComponent<AudioSource>().PlayOneShot(shutterFiring1);
            }

            if(randomShutterNoise == 1)
            {
                playerSource.GetComponent<AudioSource>().PlayOneShot(shutterFiring2);
            }

            if(randomShutterNoise == 2)
            {
                playerSource.GetComponent<AudioSource>().PlayOneShot(shutterFiring3);
            }
        }

        // Fade Alpha Value using Lerp 
        if (flashOn)
        {
            var alphaValue = image.color;
            alphaValue.a = Mathf.Lerp(minA, maxA, startA);
            image.color = alphaValue;

            startA -= 0.9f * Time.deltaTime;

            if (startA < 0f)
            {
                alphaValue.a = 0f;
                image.color = alphaValue;
                startA = 1f;
                flashOn = false;
            }
        }
    }
}
