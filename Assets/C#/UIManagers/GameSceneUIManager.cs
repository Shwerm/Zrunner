using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Manages the UI for the game scene
/// Dependencies: PlayerManager.cs, QTEManager.cs
/// </summary>
public class GameSceneUIManager : MonoBehaviour
{
    #region Singleton
    public static GameSceneUIManager gameSceneUIManagerInstance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("UI Elements")]
    [SerializeField]private GameObject qteVisual;
    [SerializeField]private Image redCircle;
    [SerializeField]private TMP_Text qteText;

    [Header("QTE Timer Settings")]
    [SerializeField]private float lerpDuration = 2f;
    #endregion

    #region Private Fields
    private PlayerManager playerManager;
    private QTEManager qteManager;

    private Vector3 startScale = new Vector3(3f, 3f, 3f);
    private Vector3 endScale = new Vector3(1f, 1f, 1f);
    private float elapsedTime = 0f;
    #endregion


    /// <summary>
    /// Initializes the GameSceneUIManager instance.
    /// </summary>
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


    /// <summary>
    /// Initializes the UI elements and references to Player Manager and QTE Manager.
    /// </summary>
    public void Start()
    {
        //Assign references to Player Manager and QTE Manager
        playerManager = PlayerManager.Instance;
        qteManager = QTEManager.Instance;

        //Error Handling
        if (playerManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize Player Manager");
        }

        if(qteManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize QTE Manager");
        }

        //Initialize UI elements to default values
        qteVisual.SetActive(false);
    }


    /// <summary>
    /// Handles the QTE visual trigger.
    /// Starts the QTE visual and handles the QTE input.
    /// Generates a random key for the QTE
    /// </summary>
    public void qteVisualTrigger()
    {
        KeyCode randomKey = GetRandomKey();
        qteText.text = randomKey.ToString();

        qteVisual.SetActive(true);

        //Start Lerp
        StartCoroutine(ShrinkImage());

        //Handlke QTE Input
        StartCoroutine(checkPlayerInput(randomKey));
    }


    /// <summary>
    /// Handles the QTE input.
    /// Checks if the player presses the correct key within the time limit.
    /// Uses the random key generated in qteVisualTrigger()
    /// </summary>
    /// <param name="randomKey"></param>
    private IEnumerator checkPlayerInput(KeyCode randomKey)
    {
        while (elapsedTime < lerpDuration)
        {
          if (Input.GetKeyDown(randomKey))
             {
                Time.timeScale = 1f;
                qteVisual.SetActive(false);
                qteManager.qteSuccess(playerManager.activeQTE);
                yield break; 
             }
          yield return null;
        }

        //If the player doesn't press the key within the time limit, handle the failure
        if(elapsedTime >= lerpDuration)
        {
            Time.timeScale = 1f;
            qteVisual.SetActive(false);
            Debug.Log("QTE Failed");
        }
    }


    /// <summary>
    /// Shrinks the red circle image over a set duration using lerp.
    /// </summary>
    private IEnumerator ShrinkImage()
    {
        //Reset elapsed time
        elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / lerpDuration);

            //Lerp between startScale and endScale based on the progress
            redCircle.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            yield return null;
        }
    }


    /// <summary>
    /// Generates a random key for the QTE.
    /// </summary>
    KeyCode GetRandomKey()
    {
        //Get all the values from the KeyCode enumeration
        KeyCode[] allKeys = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        
        //Exclude certain keys if necessary (like Escape, Mouse Buttons, etc.)
        List<KeyCode> validKeys = new List<KeyCode>();

        foreach (KeyCode key in allKeys)
        {
            //Example: Exclude mouse buttons and certain keys
            if (key >= KeyCode.A && key <= KeyCode.Z)
            {
                validKeys.Add(key);
            }
        }

        //Pick a random key from the valid keys
        int randomIndex = Random.Range(0, validKeys.Count);
        return validKeys[randomIndex];
    }
}
