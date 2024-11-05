using UnityEngine;
using UnityEditor;

public class DebugMonitorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private float updateInterval = 0.5f;
    private float lastUpdateTime;
    private float fps;
    private GUIStyle headerStyle;
    private GUIStyle valueStyle;

    [MenuItem("Window/Game Debug Monitor")]
    public static void ShowWindow()
    {
        GetWindow<DebugMonitorWindow>("Game Debug Monitor");
    }

    private void OnEnable()
    {
        headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        
        valueStyle = new GUIStyle();
        valueStyle.normal.textColor = Color.cyan;
    }

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
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("PERFORMANCE", headerStyle);
        EditorGUILayout.LabelField("FPS:", CalculateFPS().ToString("F2"), valueStyle);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("PLAYER INFO", headerStyle);
        if (PlayerManager.Instance != null)
        {
            EditorGUILayout.LabelField("Speed:", PlayerManager.Instance.moveSpeed.ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Dodge Speed Multiplier:", TimeManager.Instance.GetDodgeSpeedMultiplier().ToString("F2"), valueStyle);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("CAMERA TIMING", headerStyle);
        if (TimeManager.Instance != null)
        {
            EditorGUILayout.LabelField("Camera Tilt Multiplier:", TimeManager.Instance.GetCameraTiltMultiplier().ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Camera Hold Multiplier:", TimeManager.Instance.GetCameraHoldMultiplier().ToString("F2"), valueStyle);
        }


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("TIME & DIFFICULTY", headerStyle);
        if (TimeManager.Instance != null)
        {
            EditorGUILayout.LabelField("Survival Time:", TimeManager.Instance.playerSurvivalTime.ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Difficulty Level:", TimeManager.Instance.CurrentDifficultyLevel.ToString(), valueStyle);
        }
        else
        {
            EditorGUILayout.LabelField("TimeManager not found in scene", valueStyle);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("QTE STATUS", headerStyle);
        if (PlayerManager.Instance != null)
        {
            EditorGUILayout.LabelField("Active Parkour QTE:", PlayerManager.Instance.activeParkourQTE, valueStyle);
            EditorGUILayout.LabelField("Active Combat QTE:", PlayerManager.Instance.activeCombatQTE, valueStyle);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("QTE TIMERS", headerStyle);
        if (GameSceneUIManager.Instance != null)
        {
            EditorGUILayout.LabelField("Parkour QTE Duration:", GameSceneUIManager.Instance.parkourLerpDuration.ToString("F2"), valueStyle);
            EditorGUILayout.LabelField("Combat QTE Duration:", GameSceneUIManager.Instance.combatQteLerpDuration.ToString("F2"), valueStyle);
        }

        EditorGUILayout.EndScrollView();
        
        Repaint();
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
}
