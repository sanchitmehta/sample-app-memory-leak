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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
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
                _disposed = true;
            }
        }

        ~CPUTaskManager()
        {
            Dispose(disposing: false);
        }
    }
}