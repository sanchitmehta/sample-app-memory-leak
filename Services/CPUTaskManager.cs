using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();

        // Flag to indicate whether the object has been disposed to avoid redundant disposals
        private bool _disposed = false;

        public string StartNewTask(int complexity)
        {
            // Ensure resources are disposed when not needed anymore
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
                // Ensure proper cleanup and disposal of tasks to prevent memory leaks
                task.Stop();

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

                // Dispose tasks if they implement IDisposable
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose();
                }
            }

            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys;
        }

        // Implementing Dispose pattern for cleanup of unmanaged resources if necessary
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization because resources are already released
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    StopAllTasks();
                }

                // Note: No unmanaged resources to release in this class
                _disposed = true;
            }
        }

        ~CPUTaskManager()
        {
            Dispose(false);
        }
    }
}