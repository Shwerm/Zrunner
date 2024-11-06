using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
    private Queue<float> fpsHistory = new Queue<float>();
    private const int MAX_HISTORY_POINTS = 100;
    private Rect graphRect;
    private const float GRAPH_HEIGHT = 150f;
    private const float GRAPH_WIDTH = 400f;
    private float minFPS = float.MaxValue;
    private float maxFPS = float.MinValue;
    private float averageFPS;
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

    private void DrawPerformanceSection()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("PERFORMANCE", headerStyle);

        float currentFPS = CalculateFPS();
        UpdateFPSHistory(currentFPS);
        long memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);

        EditorGUILayout.LabelField("Current FPS:", currentFPS.ToString("F2"), valueStyle);
        EditorGUILayout.LabelField("Average FPS:", averageFPS.ToString("F2"), valueStyle);
        EditorGUILayout.LabelField("Memory Usage (MB):", memoryUsage.ToString(), valueStyle);

        DrawPerformanceGraph();
    }

    private void DrawPerformanceGraph()
    {
        graphRect = GUILayoutUtility.GetRect(GRAPH_WIDTH, GRAPH_HEIGHT);
        GUI.Box(graphRect, "");

        if (fpsHistory.Count < 2) return;

        Vector3[] linePoints = new Vector3[fpsHistory.Count];
        float[] fpsArray = fpsHistory.ToArray();
        
        for (int i = 0; i < fpsArray.Length; i++)
        {
            float x = graphRect.x + (i * graphRect.width / MAX_HISTORY_POINTS);
            // Clamp the FPS value between 0 and 60 for visualization
            float normalizedFPS = Mathf.Clamp(fpsArray[i], 0f, 200f);
            float y = graphRect.y + (graphRect.height * (1f - normalizedFPS / 200f));
            linePoints[i] = new Vector3(x, y, 0);
        }

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(2f, linePoints);
    }

    private void DrawDebugControls()
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField("DEBUG CONTROLS", headerStyle);

        if (GUILayout.Button("Reset Player Position"))
        {
            if (playerManager != null)
            {
                playerManager.transform.position = Vector3.zero;
            }
        }

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
                        timeManager.AddDebugSurvivalTime(90f);
                        break;
                    case 4:
                        timeManager.AddDebugSurvivalTime(120f);
                        break;
                }
            }
        }
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

    #region Performance Tracking
    private void UpdateFPSHistory(float currentFPS)
    {
        if (fpsHistory.Count >= MAX_HISTORY_POINTS)
        {
            fpsHistory.Dequeue();
        }
        
        fpsHistory.Enqueue(currentFPS);
        
        // Update statistics
        minFPS = Mathf.Min(minFPS, currentFPS);
        maxFPS = Mathf.Max(maxFPS, currentFPS);
        
        float sum = 0;
        foreach (float fps in fpsHistory)
        {
            sum += fps;
        }
        averageFPS = sum / fpsHistory.Count;
    }

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
