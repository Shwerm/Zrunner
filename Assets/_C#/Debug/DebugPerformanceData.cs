using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugPerformanceData : IDisposable
{
    public struct PerformanceSnapshot
    {
        public float fps;
        public float memoryUsage;
        public float timestamp;
    }

    private const int MAX_HISTORY_POINTS = 100;
    private readonly Queue<PerformanceSnapshot> performanceHistory;
    private readonly Queue<float> fpsHistory;
    private float lastUpdateTime;
    private readonly float updateInterval = 0.5f;

    public PerformanceSnapshot[] History => performanceHistory.ToArray();
    public float AverageFPS { get; private set; }
    public float MinFPS { get; private set; }
    public float MaxFPS { get; private set; }
    public float CurrentFPS { get; private set; }

    public DebugPerformanceData()
    {
        performanceHistory = new Queue<PerformanceSnapshot>(MAX_HISTORY_POINTS);
        fpsHistory = new Queue<float>(MAX_HISTORY_POINTS);
    }

    public void UpdatePerformanceData()
    {
        if (Time.unscaledTime > lastUpdateTime + updateInterval)
        {
            CurrentFPS = 1.0f / Time.unscaledDeltaTime;
            lastUpdateTime = Time.unscaledTime;
            
            AddSnapshot(CurrentFPS, UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024));
            UpdateFPSHistory(CurrentFPS);
        }
    }

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

    private void UpdateFPSHistory(float currentFPS)
    {
        if (fpsHistory.Count >= MAX_HISTORY_POINTS)
        {
            fpsHistory.Dequeue();
        }
        fpsHistory.Enqueue(currentFPS);
    }

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

    public void Dispose()
    {
        performanceHistory.Clear();
        fpsHistory.Clear();
    }
}
