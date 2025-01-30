using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Serivces
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();

        // Track whether Dispose has been called
        private bool _disposed;

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);

            // Ensure proper resource cleanup by wrapping disposable task in using statements or explicitly stopping it
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop();

                // Dispose the task after stopping to ensure proper cleanup of resources
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose();
                }
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();

                // Dispose each task to avoid memory leakage
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose();
                }
            }

            // Clear the dictionary after stopping and disposing all tasks
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys;
        }

        // Dispose method called by consumers to clean up managed and unmanaged resources
        public void Dispose()
        {
            if (!_disposed)
            {
                StopAllTasks(); // Properly stop all tasks to free up resources

                // Note: _activeTasks itself doesn't need explicit disposal
                _disposed = true;
            }
            GC.SuppressFinalize(this); // Suppress finalizer as cleanup has been completed
        }

        ~CPUTaskManager()
        {
            // Finalizer that ensures resources are freed if Dispose wasn't called
            Dispose();
        }
    }
}