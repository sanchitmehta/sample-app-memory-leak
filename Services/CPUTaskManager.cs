using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed = false;

        public string StartNewTask(int complexity)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop();
                (task as IDisposable)?.Dispose();
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            foreach (var kvp in _activeTasks)
            {
                var task = kvp.Value;
                task.Stop();
                (task as IDisposable)?.Dispose();
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            return _activeTasks.Keys;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopAllTasks();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CPUTaskManager()
        {
            Dispose(false);
        }
    }
}

public interface ICPUIntensiveTask : IDisposable
{
    void Start();
    void Stop();
}

public class CPUIntensiveTask : ICPUIntensiveTask
{
    private readonly int _complexity;
    private bool _disposed = false;
    private System.Threading.CancellationTokenSource _cancellationTokenSource;

    public CPUIntensiveTask(int complexity)
    {
        _complexity = complexity;
        _cancellationTokenSource = new System.Threading.CancellationTokenSource();
    }

    public void Start()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CPUIntensiveTask));

        // Task starting logic goes here
    }

    public void Stop()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CPUIntensiveTask));

        _cancellationTokenSource.Cancel();
        // Cleanup logic for stopping
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CPUIntensiveTask()
    {
        Dispose(false);
    }
}