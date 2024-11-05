using UnityEngine;
using UnityEditor;

/// <summary>
/// Debug monitoring window for real-time game metrics and performance analysis.
/// Provides visualization of critical game systems and performance indicators.
/// </summary>
public class DebugMonitorWindow : EditorWindow
{
    #region Cached References
    private PlayerManager playerManager;
    private TimeManager timeManager;
    private GameSceneUIManager uiManager;
    #endregion

    #region Private Fields
    private Vector2 scrollPosition;
    [SerializeField] private float updateInterval = 0.5f;
    private float lastUpdateTime;
    private float fps;
    private readonly float sectionSpacing = 10f;
    #endregion

    #region Styling
    private GUIStyle headerStyle;
    private GUIStyle valueStyle;
    [SerializeField] private Color headerColor = Color.white;
    [SerializeField] private Color valueColor = Color.cyan;
    #endregion

    #region Window Management
    [MenuItem("Window/Game Debug Monitor")]
    public static void ShowWindow()
    {
        GetWindow<DebugMonitorWindow>("Game Debug Monitor");
    }

    private void OnEnable()
    {
        InitializeStyles();
        CacheReferences();
    }

    private void OnDisable()
    {
        CleanupReferences();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes GUI styles and colors for the debug window
    /// </summary>
    private void InitializeStyles()
    {
        headerStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        headerStyle.normal.textColor = headerColor;
        
        valueStyle = new GUIStyle();
        valueStyle.normal.textColor = valueColor;
    }

    private void CacheReferences()
    {
        // Find references even if Singleton instances aren't ready
        if (playerManager == null)
        {
            playerManager = GameObject.FindObjectOfType<PlayerManager>();
        }
        if (timeManager == null)
        {
            timeManager = GameObject.FindObjectOfType<TimeManager>();
        }
        if (uiManager == null)
        {
            uiManager = GameObject.FindObjectOfType<GameSceneUIManager>();
        }
    }

    private void CleanupReferences()
    {
        playerManager = null;
        timeManager = null;
        uiManager = null;
    }

    private void OnFocus()
    {
        // Refresh references when window regains focus
        CacheReferences();
    }
    #endregion

    #region GUI Drawing
    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            DrawPlayModeDebugInfo();
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to see debug information", MessageType.Info);
        }
    }

    /// <summary>
    /// Main debug information display, organized by system
    /// </summary>
    private void DrawPlayModeDebugInfo()
    {
        try
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawPerformanceSection();
            DrawPlayerSection();
            DrawCameraSection();
            DrawTimeSection();
            DrawQTESection();

            EditorGUILayout.EndScrollView();
            Repaint();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error drawing debug info: {e.Message}");
        }
    }

    private void DrawPerformanceSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("PERFORMANCE", headerStyle);
        EditorGUILayout.LabelField("FPS:", CalculateFPS().ToString("F2"), valueStyle);
    }

    private void DrawPlayerSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("PLAYER INFO", headerStyle);
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (currentScene != "02GameScene")
        {
            EditorGUILayout.HelpBox("Switch to 02GameScene to view player data", MessageType.Info);
            return;
        }

        if (playerManager != null && playerManager.Movement != null)
        {
            EditorGUILayout.LabelField("Speed:", playerManager.Movement.moveSpeed.ToString("F2"), valueStyle);
            if (timeManager != null)
            {
                EditorGUILayout.LabelField("Dodge Speed Multiplier:", timeManager.GetDodgeSpeedMultiplier().ToString("F2"), valueStyle);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Waiting for player to initialize...", MessageType.Info);
        }
    }


    private void DrawCameraSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("CAMERA TIMING", headerStyle);
        
        if (timeManager != null)
        {
            EditorGUILayout.LabelField("Camera Tilt Multiplier:", timeManager.GetCameraTiltMultiplier().ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Camera Hold Multiplier:", timeManager.GetCameraHoldMultiplier().ToString("F2"), valueStyle);
        }
    }

    private void DrawTimeSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("TIME & DIFFICULTY", headerStyle);
        
        if (timeManager != null)
        {
            EditorGUILayout.LabelField("Survival Time:", timeManager.playerSurvivalTime.ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Difficulty Level:", timeManager.CurrentDifficultyLevel.ToString(), valueStyle);
        }
    }

    private void DrawQTESection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("QTE STATUS", headerStyle);
        
        if (playerManager != null)
        {
            EditorGUILayout.LabelField("Active Parkour QTE:", playerManager.activeParkourQTE.ToString(), valueStyle);
            EditorGUILayout.LabelField("Active Combat QTE:", playerManager.activeCombatQTE.ToString(), valueStyle);
        }

        if (uiManager != null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("QTE TIMERS", headerStyle);
            EditorGUILayout.LabelField("Parkour QTE Duration:", uiManager.parkourLerpDuration.ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Combat QTE Duration:", uiManager.combatQteLerpDuration.ToString("F2"), valueStyle);
        }
    }
    #endregion

    #region Utility Methods
    private float CalculateFPS()
    {
        if (Time.unscaledTime > lastUpdateTime + updateInterval)
        {
            fps = 1.0f / Time.unscaledDeltaTime;
            lastUpdateTime = Time.unscaledTime;
        }
        return fps;
    }
    #endregion
}
