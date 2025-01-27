using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
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
                task.Dispose();
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys.ToList();
        }

        public void Dispose()
        {
            StopAllTasks();
        }
    }
}

internal class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
{
    private bool _isRunning;

    // Simulating a complex task, actual implementation may vary
    public CPUIntensiveTask(int complexity) { /* implement complexity */ }

    public void Start()
    {
        _isRunning = true;
        // Start the task logic
    }

    public void Stop()
    {
        _isRunning = false;
        // Stop the task logic
    }

    public void Dispose()
    {
        Stop();
        // Dispose other resources if any
    }
}

internal interface ICPUIntensiveTask
{
    void Start();
    void Stop();
}