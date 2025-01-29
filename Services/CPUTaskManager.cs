using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, CPUIntensiveTask> _activeTasks = new();
        private bool _disposed;

        public string StartNewTask(int complexity)
        {
            EnsureNotDisposed();
            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            EnsureNotDisposed();
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
            EnsureNotDisposed();
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose();
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            EnsureNotDisposed();
            return _activeTasks.Keys;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopAllTasks();
            _disposed = true;
        }
    }

    public class CPUIntensiveTask : IDisposable
    {
        private readonly int _complexity;
        private byte[] _largeBuffer;
        private bool _disposed;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _largeBuffer = new byte[complexity * 1024]; // Simulated allocation
        }

        public void Start()
        {
            EnsureNotDisposed();
            // Start some CPU intensive process.
        }

        public void Stop()
        {
            EnsureNotDisposed();
            // Stop the process and release resources if necessary.
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _largeBuffer = null; // Release large buffer
            _disposed = true;
        }
    }
}