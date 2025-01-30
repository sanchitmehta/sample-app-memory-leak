using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable // Implement IDisposable to release resources
    {
        private readonly ConcurrentDictionary<string, CPUIntensiveTask> _activeTasks = new();
        private bool _disposed = false; // Track whether Dispose has been called

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);

            // Ensure appropriate cleanup in case of unexpected exceptions or scope completion
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
                // Properly dispose of the task to release resources
                task.Stop();
                task.Dispose();
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose(); // Ensure Dispose is called on each task
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys;
        }

        // Implement Dispose to free resources used by active tasks
        public void Dispose()
        {
            if (!_disposed)
            {
                StopAllTasks(); // Stop and dispose all active tasks
                _disposed = true;
            }
        }

        // Ensure objects are properly disposed of when finalizer is called
        ~CPUTaskManager()
        {
            Dispose();
        }
    }

    // Mock class to simulate CPU intensive task behavior
    internal class CPUIntensiveTask : IDisposable
    {
        private readonly int _complexity;
        private bool _isRunning;
        private bool _disposed;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (!_isRunning && !_disposed)
            {
                _isRunning = true;
                // Simulate task start logic
            }
        }

        public void Stop()
        {
            if (_isRunning && !_disposed)
            {
                _isRunning = false;
                // Simulate task stop logic
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop(); // Ensure the task is stopped before disposal
                // Release additional unmanaged resources or references if any
                _disposed = true;
            }
        }
    }
}