#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Debug monitoring window for real-time game metrics and performance analysis.
/// Provides visualization of critical game systems and performance indicators.
/// Debug monitoring window for real-time game metrics and performance analysis in the Unity Editor.
/// Provides visualization and monitoring of:
/// - Performance metrics (FPS, memory usage)
/// - Player state and movement
/// - Camera configuration
/// - Time and difficulty scaling
/// - QTE system status
/// </summary>
public class DebugMonitorWindow : EditorWindow
{
    #region Cached References
    private PlayerManager playerManager;
    private TimeManager timeManager;
    private GameSceneUIManager uiManager;
    private DebugPerformanceData performanceData;
    #endregion

    #region Private Fields
    private Vector2 scrollPosition;
    private float lastUpdateTime;
    private readonly float sectionSpacing = 10f;
    private Rect graphRect;
    private const float GRAPH_HEIGHT = 150f;
    private const float GRAPH_WIDTH = 200f;
    #endregion

    #region Styling
    private GUIStyle headerStyle;
    private GUIStyle valueStyle;
    [SerializeField] private Color headerColor = Color.white;
    [SerializeField] private Color valueColor = Color.cyan;
    #endregion


    /// <summary>
    /// Shows the debug monitor window in the Unity Editor.
    /// Creates a new window instance if one doesn't exist.
    /// </summary>
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
        performanceData = new DebugPerformanceData();
    }

    private void OnDisable()
    {
        CleanupReferences();
    }

    private void OnFocus()
    {
        CacheReferences();
    }
    #endregion


    #region Initialization
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

    /// <summary>
    /// Initializes and caches references to critical game systems.
    /// Finds singleton instances of managers needed for debugging.
    /// </summary>
    private void CacheReferences()
    {
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

    private void DrawPlayModeDebugInfo()
    {
        try
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawPerformanceSection();
            DrawDebugControls();
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

    /// <summary>
    /// Draws real-time performance metrics including FPS and memory usage.
    /// Updates and displays current performance snapshot data.
    /// </summary>
    private void DrawPerformanceSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("PERFORMANCE", headerStyle);

        performanceData.UpdatePerformanceData();
        long memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);

        EditorGUILayout.LabelField("Current FPS:", performanceData.CurrentFPS.ToString("F2"), valueStyle);
        EditorGUILayout.LabelField("Average FPS:", performanceData.AverageFPS.ToString("F2"), valueStyle);
        EditorGUILayout.LabelField("Memory Usage (MB):", memoryUsage.ToString(), valueStyle);

        DrawPerformanceGraph();
    }

    /// <summary>
    /// Draws the performance monitoring graph showing FPS history over time.
    /// Visualizes data points from the performance history buffer.
    /// </summary>
    private void DrawPerformanceGraph()
    {
        graphRect = GUILayoutUtility.GetRect(GRAPH_WIDTH, GRAPH_HEIGHT);
        GUI.Box(graphRect, "");

        var history = performanceData.History;
        if (history.Length < 2) return;

        Vector3[] linePoints = new Vector3[history.Length];
        
        for (int i = 0; i < history.Length; i++)
        {
            float x = graphRect.x + (i * graphRect.width / DebugConstants.MAX_HISTORY_POINTS);
            float normalizedFPS = Mathf.Clamp(history[i].fps, 0f, 200f);
            float y = graphRect.y + (graphRect.height * (1f - normalizedFPS / 200f));
            linePoints[i] = new Vector3(x, y, 0);
        }

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(2f, linePoints);
    }

    /// <summary>
    /// Renders debug controls for runtime game manipulation.
    /// Includes player position reset and difficulty level forcing.
    /// </summary>
    private void DrawDebugControls()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("DEBUG CONTROLS", headerStyle);


        if (timeManager != null)
        {
            if (GUILayout.Button("Force Next Difficulty Level"))
            {
                switch (timeManager.CurrentDifficultyLevel)
                {
                    case 1:
                        timeManager.AddDebugSurvivalTime(30f);
                        break;
                    case 2:
                        timeManager.AddDebugSurvivalTime(60f);
                        break;
                    case 3:
                        timeManager.AddDebugSurvivalTime(120f);
                        break;
                    case 4:
                        timeManager.AddDebugSurvivalTime(200f);
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Renders player state information when in the game scene.
    /// Displays movement speeds and active modifiers.
    /// </summary>
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
}
#endif