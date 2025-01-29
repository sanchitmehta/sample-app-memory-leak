using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose();
                }
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUTaskManager));

            return _activeTasks.Keys.ToList();
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
}