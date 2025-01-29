using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _isDisposed; // Track whether Dispose has been called.

        public string StartNewTask(int complexity)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(CPUTaskManager));

            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(CPUTaskManager));

            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop();
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose(); // Properly dispose of disposable tasks.
                }
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(CPUTaskManager));

            foreach (var pair in _activeTasks)
            {
                pair.Value.Stop();
                if (pair.Value is IDisposable disposableTask)
                {
                    disposableTask.Dispose(); // Properly dispose of disposable tasks.
                }
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(CPUTaskManager));
            return _activeTasks.Keys.ToList(); // Ensure thread-safe enumeration of keys.
        }

        // Implement the Dispose pattern to avoid memory/resource leaks.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization for this object.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return; // Avoid re-running dispose logic.
            if (disposing)
            {
                StopAllTasks(); // Ensure all tasks are stopped and resources freed.
            }
            _isDisposed = true;
        }

        ~CPUTaskManager()
        {
            Dispose(false); // Finalizer calls Dispose in case Dispose was not called explicitly.
        }
    }

    public interface ICPUIntensiveTask
    {
        void Start();
        void Stop();
        // Assuming some disposable resources used in tasks.
    }

    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private CancellationTokenSource _cancellationTokenSource;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _cancellationTokenSource = new CancellationTokenSource(); // Initialize disposable resource.
        }

        public void Start()
        {
            // Simulate starting a resource-intensive operation.
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel(); // Cancel any running tasks.
        }

        public void Dispose()
        {
            // Properly dispose of any resources here.
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null; // Prevent further usage.
            GC.SuppressFinalize(this); // Suppress finalization for this object.
        }

        ~CPUIntensiveTask()
        {
            Dispose();
        }
    }
}