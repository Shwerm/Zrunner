using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSceneUIManager : MonoBehaviour
{
    //Define Singleton instance
    public static GameSceneUIManager gameSceneUIManagerInstance { get; private set; }

    private PlayerManager playerManager;
    private QTEManager qteManager;

    public GameObject qteVisual;
    public Image redCircle;
    public TMP_Text qteText;

    //Circle Lerp Variables
    public float lerpDuration = 2f;
    private Vector3 startScale = new Vector3(3f, 3f, 3f);
    private Vector3 endScale = new Vector3(1f, 1f, 1f);
    private float elapsedTime = 0f;


    //Create Singleton Instance of the Game Scene UIManager
    private void Awake()
    {
        if (gameSceneUIManagerInstance == null)
        {
            gameSceneUIManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        playerManager = PlayerManager.playerManagerInstance;
        qteManager = QTEManager.QTEManagerInstance;
        if (playerManager == null)
        {
            Debug.LogError("Failed to initialize Player Manager");
        }

        if(qteManager == null)
        {
            Debug.LogError("Failed to initialize QTE Manager");
        }

        qteVisual.SetActive(false);
    }



    public void qteVisualTrigger()
    {
        KeyCode randomKey = GetRandomKey();
        qteText.text = randomKey.ToString();

        qteVisual.SetActive(true);

        //Start Lerp
        StartCoroutine(ShrinkImage());

        //Handlke QTE Success
        StartCoroutine(checkPlayerInput(randomKey));

        //Handle QTE Failure
    }

    private IEnumerator checkPlayerInput(KeyCode randomKey)
    {
        while (true)
        {
          if (Input.GetKeyDown(randomKey))
             {
                Time.timeScale = 1f;
                qteVisual.SetActive(false);
                qteManager.qteSuccess(playerManager.activeQTE);
                yield break; // This terminates the coroutine
             }
          yield return null; // Wait for the next frame
         }
    }


    private IEnumerator ShrinkImage()
    {
        // Reset elapsed time
        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time
            float progress = Mathf.Clamp01(elapsedTime / lerpDuration); // Progress from 0 to 1

            // Lerp between startScale and endScale based on the progress
            redCircle.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            yield return null; // Wait for the next frame
        }
    }

    KeyCode GetRandomKey()
    {
        // Get all the values from the KeyCode enumeration
        KeyCode[] allKeys = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        
        // Exclude certain keys if necessary (like Escape, Mouse Buttons, etc.)
        List<KeyCode> validKeys = new List<KeyCode>();

        foreach (KeyCode key in allKeys)
        {
            // Example: Exclude mouse buttons and certain keys
            if (key >= KeyCode.A && key <= KeyCode.Z)
            {
                validKeys.Add(key);
            }
        }

        // Pick a random key from the valid keys
        int randomIndex = Random.Range(0, validKeys.Count);
        return validKeys[randomIndex];
    }
}
