using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the pause menu and pause functionality in the game.
/// 
/// Dependencies: GameManager.cs
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private GameManager gameManager;
    
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get the GameManager instance
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("[PauseManager] GameManager not assigned");
            return;
        }
        // Ensure the pause menu is initially hidden
        pauseMenuUI.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "02GameScene") return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Toggles the pause state and updates the pause menu UI accordingly.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Quits the game and returns to the main menu.
    /// </summary>
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        gameManager.sceneLoader("01MainMenu");
    }
}
