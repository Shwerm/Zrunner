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
    private CombatQTEManager combatQTEManager;

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
        combatQTEManager = CombatQTEManager.Instance;

        //Error Handling
        if (playerManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize Player Manager");
        }

        if(parkourQTEManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize  Parkour QTE Manager");
        }

        if(combatQTEManager == null)
        {
            Debug.LogError("[GameSceneUIManager] Failed to initialize Combat QTE Manager");
        }
        //Initialize UI elements to default values
        parkourQTEVisual.SetActive(false);
        combatQteTimerVisual.SetActive(false);
        combatQteLeftGreen.SetActive(false);
        combatQteRightGreen.SetActive(false);
    }





    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    #region Parkour QTE
    /// <summary>
    /// Handles the QTE visual trigger.
    /// Starts the QTE visual and handles the QTE input.
    /// Generates a random key for the QTE
    /// </summary>
    public void parkourQteVisualTrigger()
    {
        parkourQteText.text = parkourQTEManager.randomKey.ToString();

        parkourQTEVisual.SetActive(true);

        //Start Lerp
        StartCoroutine(ShrinkRedCircle());

        //Handlke QTE Input
        StartCoroutine(checkPlayerKeyBoardInput(parkourQTEManager.randomKey));
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
    #endregion


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    #region Combat QTE
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
    /// Shrinks the red bar over a set duration using lerp.
    /// If the QTE was completed successfully, the UI elements are hidden.
    /// If the QTE was not completed successfully, the UI elements are hidden
    /// and player death state is triggered.
    /// </summary>
    private IEnumerator ShrinkRedBar()
    {
        elapsedTime = 0f;
        float unscaledLerpDuration = combatQteLerpDuration * 0.5f;

        while (elapsedTime < unscaledLerpDuration)
        {
            // Check if QTE was completed successfully (UI elements are hidden)
            if (!combatQteTimerVisual.activeSelf)
            {
                yield break;
            }

            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / unscaledLerpDuration);
            combatQteTimeBar.fillAmount = Mathf.Lerp(1f, 0f, progress);
          
            if (progress >= 1f && combatQteTimerVisual.activeSelf)
            {
                combatQteTimerVisual.SetActive(false);
                combatQteLeftGreen.SetActive(false);
                combatQteRightGreen.SetActive(false);
                Debug.Log("Combat QTE Failed");
                combatQTEManager.RushEnemyToPlayer(playerManager.activeCombatQTE);
            }
          
            yield return null;
        }
    }

    //Button OnClick functions for left and right side squares
    public void LeftSquareClick()
    {
        Debug.Log("Lefty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteLeftGreen.SetActive(false);
        StartCoroutine(combatQTEManager.ShootEnemy(playerManager.activeCombatQTE));
    }

    public void RightSquareClick()
    {
        Debug.Log("Righty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteRightGreen.SetActive(false);
        StartCoroutine(combatQTEManager.ShootEnemy(playerManager.activeCombatQTE));
    }
    #endregion
}
