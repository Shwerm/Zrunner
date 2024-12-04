using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI for the game scene
/// Handles all QTE Indicators and timers through UI elements.
/// 
/// Dependencies: PlayerManager.cs, ParkourQTEManager.cs, CombatQTEManager.cs
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
    [SerializeField]public float parkourLerpDuration = 4f;

    [Header("Combat QTE Timer Settings")]
    [SerializeField]public float combatQteLerpDuration = 4f;
    #endregion


    #region Private Fields
    private PlayerManager playerManager;
    private ParkourQTEManager parkourQTEManager;
    private CombatQTEManager combatQTEManager;

    private Vector3 startScale = new Vector3(4f, 4f, 4f);
    private Vector3 endScale = new Vector3(1.5f, 1.5f, 1.5f);
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
    /// Initializes the UI elements and references to Player Manager and QTE Managers.
    /// </summary>
    public void Start()
    {
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
    /// Handles the parkour QTE visual trigger.
    /// Starts the parkour QTE visual and handles the parkour QTE input.
    /// Generates a random key for the parkour QTE
    /// </summary>
    public void parkourQteVisualTrigger()
    {
        parkourQteText.text = parkourQTEManager.randomKey.ToString();

        parkourQTEVisual.SetActive(true);

        //Start Lerp
        StartCoroutine(ShrinkRedCircle());

        //Handle parkour QTE Input
        StartCoroutine(checkPlayerKeyBoardInput(parkourQTEManager.randomKey));
    }

    /// <summary>
    /// Handles the parkour QTE input.
    /// Checks if the player presses the correct key within the time limit.
    /// Uses the random key generated in qteVisualTrigger().
    /// </summary>
    /// <param name="randomKey"></param>
    private IEnumerator checkPlayerKeyBoardInput(KeyCode randomKey)
    {
        while (elapsedTime < parkourLerpDuration)
        {
            //Only check for input if game is not paused
            if (Time.timeScale != 0)
            {
                if (Input.GetKeyDown(randomKey))
                {
                    Time.timeScale = 1f;
                    parkourQTEVisual.SetActive(false);
                    parkourQTEManager.parkourQteSuccess(playerManager.activeParkourQTE);
                    yield break; 
                }
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
        elapsedTime = 0f;
        float currentDuration = parkourLerpDuration;

        while (elapsedTime < currentDuration)
        {
            //Only increment time if game is not paused
            if (Time.timeScale != 0)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
            
            float progress = elapsedTime / currentDuration;
            redCircle.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            
            if (progress >= 1f)
            {
                redCircle.transform.localScale = endScale;
                break;
            }
            
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
    /// If the combat QTE was completed successfully, the UI elements are hidden.
    /// If the combat QTE was not completed successfully, the UI elements are hidden
    /// and player death state is triggered.
    /// </summary>
    private IEnumerator ShrinkRedBar()
    {
        elapsedTime = 0f;
        float currentDuration = combatQteLerpDuration;

        while (elapsedTime < currentDuration)
        {
            if (!combatQteTimerVisual.activeSelf)
            {
                yield break;
            }

            //Only increment time if game is not paused
            if (Time.timeScale != 0)
            {
                elapsedTime += Time.unscaledDeltaTime;
            }
                
            float progress = elapsedTime / currentDuration;
            combatQteTimeBar.fillAmount = 1f - progress;
                
            if (progress >= 1f && combatQteTimerVisual.activeSelf)
            {
                combatQteTimerVisual.SetActive(false);
                combatQteLeftGreen.SetActive(false);
                combatQteRightGreen.SetActive(false);
                combatQTEManager.RushEnemyToPlayer(playerManager.activeCombatQTE);
                break;
            }
                
            yield return null;
        }
    }



    //Button OnClick functions for left and right side squares
    public void LeftSquareClick()
    {
        if (Time.timeScale == 0) return;
        
        Debug.Log("Lefty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteLeftGreen.SetActive(false);
        StartCoroutine(combatQTEManager.ShootEnemy(playerManager.activeCombatQTE));
    }


    public void RightSquareClick()
    {
        if (Time.timeScale == 0) return;
        
        Debug.Log("Righty Clicked");
        Time.timeScale = 1f;
        combatQteTimerVisual.SetActive(false);
        combatQteRightGreen.SetActive(false);
        StartCoroutine(combatQTEManager.ShootEnemy(playerManager.activeCombatQTE));
    }
    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    #region Difficulty Scaling
    private void OnEnable()
    {
        StartCoroutine(SubscribeToTimeManager());
    }

    private IEnumerator SubscribeToTimeManager()
    {
        yield return new WaitForEndOfFrame();
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDifficultyChanged += UpdateLerpDurations;
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDifficultyChanged -= UpdateLerpDurations;
    }

    private void UpdateLerpDurations(int difficultyLevel)
    {
        float multiplier = TimeManager.Instance.CurrentLerpMultiplier;
        
        //Store original base values as constants
        const float BASE_PARKOUR_DURATION = 4f;
        const float BASE_COMBAT_DURATION = 4f;
        
        //Apply multiplier to fresh base values each time
        parkourLerpDuration = BASE_PARKOUR_DURATION * multiplier;
        combatQteLerpDuration = BASE_COMBAT_DURATION * multiplier;
        
        Debug.Log($"[GameSceneUIManager] Updated QTE Durations - Parkour: {parkourLerpDuration:F2}s, Combat: {combatQteLerpDuration:F2}s");
    }

    #endregion
}
