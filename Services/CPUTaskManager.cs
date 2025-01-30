using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable // Adding IDisposable to properly handle resource cleanup
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed = false; // To track whether Dispose has been called

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();

            // CPUIntensiveTask implements IDisposable, so ensure it is properly disposed (improvement suggested in comments)
            var task = new CPUIntensiveTask(complexity);
            try
            {
                task.Start();
                _activeTasks.TryAdd(taskId, task);
            }
            catch
            {
                // Ensure proper cleanup in case of failure to start the task
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
                task.Dispose(); // Ensure the task is properly disposed to release resources like buffers
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            foreach (var taskId in _activeTasks.Keys)
            {
                if (_activeTasks.TryRemove(taskId, out var task))
                {
                    task.Stop();
                    task.Dispose(); // Dispose tasks to avoid memory leaks
                }
            }
        }

        public IEnumerable<string> GetActiveTasks()
        {
            // No changes needed here as strings are returned by reference, ensure methods using them don't hold for long.
            return _activeTasks.Keys;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    StopAllTasks(); // This clears all tasks and disposes them
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Dispose logic implemented as per the standard pattern
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization as resources are cleaned up
        }
    }
}