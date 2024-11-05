using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugPerformanceData
{
    public struct PerformanceSnapshot
    {
        public float fps;
        public float memoryUsage;
        public float timestamp;
    }

    private const int MAX_HISTORY_POINTS = 100;
    private Queue<PerformanceSnapshot> performanceHistory;

    public PerformanceSnapshot[] History => performanceHistory.ToArray();
    public float AverageFPS { get; private set; }
    public float MinFPS { get; private set; }
    public float MaxFPS { get; private set; }

    public DebugPerformanceData()
    {
        performanceHistory = new Queue<PerformanceSnapshot>(MAX_HISTORY_POINTS);
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
}
