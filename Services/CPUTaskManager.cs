using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PerformanceIssues.Serivces
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
            if (!_activeTasks.TryAdd(taskId, task))
            {
                task.Dispose();
            }
            return taskId;
        }

        public bool StopTask(string taskId)
        {
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
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose();
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
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
}