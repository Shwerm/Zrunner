//using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core performance monitoring and analysis for tracking real-time debug data.
/// Provides analysis of:
/// - FPS tracking with min/max/average calculations
/// - Memory allocation monitoring
/// - Historical performance snapshots
/// - Timestamp-based trend analysis
/// - Configurable data buffer management
/// - Interval-based metric updates
/// </summary>
public static class DebugConstants
{
    public const int MAX_HISTORY_POINTS = 100;
}

public class DebugPerformanceData
{
    /// <summary>
    /// Stores a single frame's performance metrics including FPS, memory usage and timestamp.
    /// Used for historical tracking and trend analysis for perfromance graph.
    /// </summary>
    #region Data Structures
    public struct PerformanceSnapshot
    {
        public float fps;
        public float memoryUsage;
        public float timestamp;
    }
    #endregion

    #region Constants
    private const int MAX_HISTORY_POINTS = DebugConstants.MAX_HISTORY_POINTS;
    private const float UPDATE_INTERVAL = 0.5f;
    #endregion

    #region Private Fields
    private Queue<PerformanceSnapshot> performanceHistory;
    private float lastUpdateTime;
    private float currentFPS;
    #endregion

    #region Public Properties
    public PerformanceSnapshot[] History => performanceHistory.ToArray();
    public float AverageFPS { get; private set; }
    public float MinFPS { get; private set; }
    public float MaxFPS { get; private set; }
    public float CurrentFPS => currentFPS;
    #endregion

    public DebugPerformanceData()
    {
        performanceHistory = new Queue<PerformanceSnapshot>(MAX_HISTORY_POINTS);
        lastUpdateTime = Time.unscaledTime;
    }

    #region Public Methods
    /// <summary>
    /// Updates performance data based on the configured interval.
    /// Captures current FPS and memory usage when interval is met.
    /// </summary>
    public void UpdatePerformanceData()
    {
        if (Time.unscaledTime > lastUpdateTime + UPDATE_INTERVAL)
        {
            currentFPS = 1.0f / Time.unscaledDeltaTime;
            lastUpdateTime = Time.unscaledTime;
            
            long memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            AddSnapshot(currentFPS, memoryUsage);
        }
    }

    /// <summary>
    /// Adds a new performance snapshot to the historical buffer.
    /// Maintains maximum history size by removing oldest entries when full.
    /// </summary>
    /// <param name="fps">Current frames per second</param>
    /// <param name="memoryUsage">Current memory usage in Mb</param>
    public void AddSnapshot(float fps, float memoryUsage)
    {
        if (performanceHistory.Count >= MAX_HISTORY_POINTS)
        {
            performanceHistory.Dequeue();
        }

        performanceHistory.Enqueue(new PerformanceSnapshot
        {
            fps = fps,
            memoryUsage = memoryUsage,
            timestamp = Time.realtimeSinceStartup
        });

        UpdateStats();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Recalculates performance statistic from the snapshot history.
    /// Updates min, max and average FPS values.
    /// </summary>
    private void UpdateStats()
    {
        float sum = 0;
        MinFPS = float.MaxValue;
        MaxFPS = float.MinValue;

        foreach (var snapshot in performanceHistory)
        {
            sum += snapshot.fps;
            MinFPS = Mathf.Min(MinFPS, snapshot.fps);
            MaxFPS = Mathf.Max(MaxFPS, snapshot.fps);
        }

        AverageFPS = sum / performanceHistory.Count;
    }
    #endregion
}
