using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable // Ensure the class implements IDisposable to clean up disposable resources
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed = false; // Track whether the object has been disposed

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            // Properly manage the lifecycle of CPUIntensiveTask as it likely holds unmanaged resources
            var task = new CPUIntensiveTask(complexity);
            try
            {
                task.Start();
                _activeTasks.TryAdd(taskId, task);
            }
            catch
            {
                // Clean up task if starting fails
                task.Dispose();
                throw;
            }
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop();
                task.Dispose(); // Ensure the task is disposed after it is stopped
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose(); // Dispose each task to release resources
            }
            _activeTasks.Clear(); // Clear the dictionary after stopping and disposing the tasks
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys;
        }

        // Implement IDisposable to clean up all tasks and other resources
        public void Dispose()
        {
            if (!_disposed)
            {
                StopAllTasks(); // Ensure all active tasks are stopped and disposed
                _disposed = true;
            }
        }
    }
}