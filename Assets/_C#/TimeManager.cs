using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    #region Singleton
    public static TimeManager Instance { get; private set; }
    #endregion

    #region Events
    public event Action<int> OnDifficultyChanged;
    #endregion

    #region Difficulty Constants
    private const float LEVEL_2_THRESHOLD = 30f;
    private const float LEVEL_3_THRESHOLD = 60f;
    private const float LEVEL_4_THRESHOLD = 120f;
    private const float LEVEL_5_THRESHOLD = 200f;
    #endregion

    #region Difficulty Multipliers
    [Header("QTE Difficulty Multipliers")]
    [SerializeField] private float[] speedMultipliers = { 1f, 1.2f, 1.4f, 1.6f, 1f };
    [SerializeField] private float[] lerpDurationMultipliers = { 1f, 0.8f, 0.6f, 0.4f, 0.3f };

    [Header("Player Movement Difficulty Multipliers")]
    [SerializeField] private float[] dodgeSpeedMultipliers = { 1f, 1f, 1.3f, 1.6f, 3f }; 

    [Header("Camera Multipliers")]
    [SerializeField] private float[] cameraTiltMultipliers = { 1f, 1f, 0.8f, 0.6f, 0.5f }; 
    [SerializeField] private float[] cameraHoldMultipliers = { 1f, 1f, 0.7f, 0.6f, 1f }; 

    #endregion

    #region Private Fields
    private GameManager gameManager;
    private int currentDifficultyLevel = 1;
    #endregion

    #region Public Properties
    public float playerSurvivalTime { get; private set; } = 0f;
    public int CurrentDifficultyLevel => currentDifficultyLevel;
    public float CurrentSpeedMultiplier => speedMultipliers[currentDifficultyLevel - 1];
    public float CurrentLerpMultiplier => lerpDurationMultipliers[currentDifficultyLevel - 1];
    #endregion

    #region Debug Properties
    [SerializeField, Header("Debug Info")]
    private bool showDebugInfo = true;

    public bool IsDebugEnabled => showDebugInfo;
    public string DebugSurvivalTime => playerSurvivalTime.ToString("F2");
    public string DebugDifficultyLevel => currentDifficultyLevel.ToString();
    #endregion


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

    private void Start()
    {
        gameManager = GameManager.Instance;
        ResetDifficulty();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "02GameScene")
        {
            ResetDifficulty();
        }
    }

    private void Update()
    {
        playerSurvivalTime += Time.deltaTime;
        gameManager.playerScore = playerSurvivalTime;
        UpdateDifficultyLevel();
    }

    private void UpdateDifficultyLevel()
    {
        int newLevel = CalculateDifficultyLevel();
        
        if (newLevel != currentDifficultyLevel)
        {
            currentDifficultyLevel = newLevel;
            OnDifficultyChanged?.Invoke(currentDifficultyLevel);
            Debug.Log($"[TimeManager] Difficulty increased to Level {currentDifficultyLevel}");
        }
    }

    private int CalculateDifficultyLevel()
    {
        if (playerSurvivalTime >= LEVEL_5_THRESHOLD) return 5;
        if (playerSurvivalTime >= LEVEL_4_THRESHOLD) return 4;
        if (playerSurvivalTime >= LEVEL_3_THRESHOLD) return 3;
        if (playerSurvivalTime >= LEVEL_2_THRESHOLD) return 2;
        return 1;
    }

    public void ResetDifficulty()
    {
        playerSurvivalTime = 0f;
        currentDifficultyLevel = 1;
        OnDifficultyChanged?.Invoke(currentDifficultyLevel);
        Debug.Log("[TimeManager] Difficulty reset to Level 1");
    }

    public float GetDodgeSpeedMultiplier()
    {
        return dodgeSpeedMultipliers[currentDifficultyLevel - 1];
    }

    public float GetCameraTiltMultiplier()
    {
        return cameraTiltMultipliers[currentDifficultyLevel - 1];
    }

    public float GetCameraHoldMultiplier()
    {
        return cameraHoldMultipliers[currentDifficultyLevel - 1];
    }

    public void AddDebugSurvivalTime(float timeToAdd)
    {
        playerSurvivalTime = timeToAdd;
        UpdateDifficultyLevel();
    }

}