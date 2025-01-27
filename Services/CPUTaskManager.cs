using System;
using System.Collections.Concurrent;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool disposedValue;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopAllTasks();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}