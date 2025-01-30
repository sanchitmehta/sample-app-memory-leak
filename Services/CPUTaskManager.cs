using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed;

        // Start and manage a computationally intensive task.
        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            // Ensure CPUIntensiveTask implements IDisposable, if it uses unmanaged resources.
            var task = new CPUIntensiveTask(complexity);
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        // Stop and properly clean up resources when a task is stopped.
        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop(); // Ensure proper cleanup logic is present inside Stop.
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose(); // Dispose of task explicitly if it implements IDisposable.
                }
                return true;
            }
            return false;
        }

        // Stop all tasks and free up resources.
        public void StopAllTasks()
        {
            foreach (var task in _activeTasks.Values)
            {
                task.Stop(); // Ensure proper cleanup logic is present in Stop.
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose(); // Explicitly dispose of each task if it supports IDisposable.
                }
            }
            _activeTasks.Clear(); // Clear dictionary after cleanup to prevent references remaining in memory.
        }

        // Retrieve active tasks for monitoring purposes.
        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys; // Return task identifiers as this does not retain references to the tasks themselves.
        }

        // Implement the IDisposable pattern to ensure CPUTaskManager properly cleans up when no longer in use.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free managed resources, including active running tasks.
                    StopAllTasks();
                }

                // Free unmanaged resources here, if any.

                _disposed = true;
            }
        }

        // Dispose method to explicitly release resources when the manager is no longer in use.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Ensure a finalizer is available for cleanup, just in case Dispose is not called explicitly.
        ~CPUTaskManager()
        {
            Dispose(false);
        }
    }
}