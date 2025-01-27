using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed;

        public string StartNewTask(int complexity)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);
            
            try
            {
                task.Start();
                if (!_activeTasks.TryAdd(taskId, task))
                {
                    task.Stop(); 
                }
            }
            catch
            {
                task.Dispose(); 
                throw;
            }
            
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

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
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose();
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
    }

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private readonly int _complexity;
        private bool _disposed;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Start task logic here
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            // Stop task logic here
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                }
                // Release unmanaged resources here
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}