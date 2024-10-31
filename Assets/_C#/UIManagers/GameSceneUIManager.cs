using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


/// <summary>
/// Manages the UI for the game scene
/// Dependencies: PlayerManager.cs, QTEManager.cs
/// </summary>
public class GameSceneUIManager : MonoBehaviour
{
    #region Singleton
    public static GameSceneUIManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("UI Elements")]
    [SerializeField]private GameObject parkourQTEVisual;
    [SerializeField]private Image redCircle;
    [SerializeField]private TMP_Text parkourQteText;

    [SerializeField]private GameObject combatQteTimerVisual;
    [SerializeField]private Image combatQteTimeBar;
    [SerializeField]private GameObject combatQteLeftGreen;
    [SerializeField]private GameObject combatQteRightGreen;

    [Header("Parkour QTE Timer Settings")]
    [SerializeField]private float parkourLerpDuration = 2f;

    [Header("Combat QTE Timer Settings")]
    [SerializeField]private float combatQteLerpDuration = 0.5f;
    #endregion

    #region Private Fields
    private PlayerManager playerManager;
    private ParkourQTEManager parkourQTEManager;

    private Vector3 startScale = new Vector3(3f, 3f, 3f);
    private Vector3 endScale = new Vector3(1f, 1f, 1f);
    private float elapsedTime = 0f;
    #endregion


    /// <summary>
    /// Initializes the GameSceneUIManager instance.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        parkourQTEManager = ParkourQTEManager.Instance;

        //Error Handling
        if (playerManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize Player Manager");
        }

        if(parkourQTEManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize QTE Manager");
        }

        //Initialize UI elements to default values
        parkourQTEVisual.SetActive(false);
        combatQteTimerVisual.SetActive(false);
        combatQteLeftGreen.SetActive(false);
        combatQteRightGreen.SetActive(false);
    }


    /// <summary>
    /// Handles the QTE visual trigger.
    /// Starts the QTE visual and handles the QTE input.
    /// Generates a random key for the QTE
    /// </summary>
    public void parkourQteVisualTrigger()
    {
        KeyCode randomKey = GetRandomKey();
        parkourQteText.text = randomKey.ToString();

        parkourQTEVisual.SetActive(true);

        //Start Lerp
        StartCoroutine(ShrinkRedCircle());

        //Handlke QTE Input
        StartCoroutine(checkPlayerKeyBoardInput(randomKey));
    }

    public void combatQteVisualTrigger(string activeCombatQTE)
    {
        combatQteTimerVisual.SetActive(true);
        if(activeCombatQTE == "Left")
        {
            combatQteLeftGreen.SetActive(true);
        }
        else if (activeCombatQTE == "Right")
        {
            combatQteRightGreen.SetActive(true);
        }

        //Start Lerp
        StartCoroutine(ShrinkRedBar());
    }


    /// <summary>
    /// Handles the Parkour QTE input.
    /// Checks if the player presses the correct key within the time limit.
    /// Uses the random key generated in qteVisualTrigger()
    /// </summary>
    /// <param name="randomKey"></param>
    private IEnumerator checkPlayerKeyBoardInput(KeyCode randomKey)
    {
        while (elapsedTime < parkourLerpDuration)
        {
          if (Input.GetKeyDown(randomKey))
             {
                Time.timeScale = 1f;
                parkourQTEVisual.SetActive(false);
                parkourQTEManager.parkourQteSuccess(playerManager.activeParkourQTE);
                yield break; 
             }
          yield return null;
        }

        //If the player doesn't press the key within the time limit, handle the failure
        if(elapsedTime >= parkourLerpDuration)
        {
            Time.timeScale = 1f;
            parkourQTEVisual.SetActive(false);
            Debug.Log("QTE Failed");
        }
    }


    public void LeftSquareClick()
    {
        Debug.Log("Lefty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteLeftGreen.SetActive(false);
    }

    public void RightSquareClick()
    {
        Debug.Log("Righty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteRightGreen.SetActive(false);
    }


    /// <summary>
    /// Shrinks the red circle image over a set duration using lerp.
    /// </summary>
    private IEnumerator ShrinkRedCircle()
    {
        //Reset elapsed time
        elapsedTime = 0f;

        while (elapsedTime < parkourLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / parkourLerpDuration);

            //Lerp between startScale and endScale based on the progress
            redCircle.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            yield return null;
        }
    }


    private IEnumerator ShrinkRedBar()
    {
        elapsedTime = 0f;
        float unscaledLerpDuration = combatQteLerpDuration * 0.5f;

        while (elapsedTime < unscaledLerpDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / unscaledLerpDuration);
            combatQteTimeBar.fillAmount = Mathf.Lerp(1f, 0f, progress);
            
            if (progress >= 1f)
            {
                combatQteTimerVisual.SetActive(false);
                combatQteLeftGreen.SetActive(false);
                combatQteRightGreen.SetActive(false);
                Debug.Log("Combat QTE Failed");
            }
            
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
