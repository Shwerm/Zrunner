using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// Debug monitoring window for real-time game metrics and performance analysis in the Unity Editor.
/// Provides visualization and monitoring of:
/// - Performance metrics (FPS, memory usage)
/// - Player state and movement
/// - Camera configuration
/// - Time and difficulty scaling
/// - QTE system status
/// </summary>
/// <remarks>
/// Access via Window > Game Debug Monitor in the Unity Editor.
/// Only functions during Play Mode in the '02GameScene'.
/// </remarks>

// Previous using statements remain the same

public class DebugMonitorWindow : EditorWindow, IDisposable
{
    #region Private Fields
    private Vector2 scrollPosition;
    private readonly float sectionSpacing = 10f;
    private Rect graphRect;
    private const float GRAPH_HEIGHT = 150f;
    private const float GRAPH_WIDTH = 400f;
    private readonly DebugMonitorStyles styles;
    private readonly DebugPerformanceData performanceData;
    private readonly GUIContent[] headerContents;
    #endregion

    #region Cached References
    private PlayerManager playerManager;
    private TimeManager timeManager;
    private GameSceneUIManager uiManager;
    #endregion

    public DebugMonitorWindow()
    {
        styles = new DebugMonitorStyles();
        performanceData = new DebugPerformanceData();
        headerContents = InitializeHeaderContents();
    }

    private GUIContent[] InitializeHeaderContents()
    {
        return new[]
        {
            new GUIContent("PERFORMANCE"),
            new GUIContent("DEBUG CONTROLS"),
            new GUIContent("PLAYER INFO"),
            new GUIContent("CAMERA TIMING"),
            new GUIContent("TIME & DIFFICULTY"),
            new GUIContent("QTE STATUS")
        };
    }

    // Previous MenuItem and OnEnable methods remain

    private void OnEnable()
    {
        styles.Initialize();
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (!Application.isPlaying) return;
        
        playerManager ??= GameObject.FindObjectOfType<PlayerManager>();
        timeManager ??= GameObject.FindObjectOfType<TimeManager>();
        uiManager ??= GameObject.FindObjectOfType<GameSceneUIManager>();
    }

    private void DrawSection(string header, Action drawContent)
    {
        EditorGUILayout.Space(sectionSpacing);
        EditorGUILayout.LabelField(header, styles.HeaderStyle);
        
        SafeDrawSection(header, drawContent);
    }

    private void SafeDrawSection(string sectionName, Action drawAction)
    {
        try
        {
            drawAction?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error drawing {sectionName} section: {ex.Message}");
            EditorGUILayout.HelpBox($"Failed to draw {sectionName} section", MessageType.Error);
        }
    }

    // Implement remaining drawing methods using DrawSection and SafeDrawSection
    // Previous drawing methods remain but use the new helper methods

    public void Dispose()
    {
        performanceData.Dispose();
        CleanupReferences();
    }
}

