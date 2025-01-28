using System.Collections.Concurrent;
using System;

namespace PerformanceIssues.Services
{
    public class CPUTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();

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
                task.Dispose(); // Ensure the task's resources are released
                return true;
            }
            return false;
        }

        public void StopAllTasks()
        {
            foreach (var task in _activeTasks.Values)
            {
                task.Stop();
                task.Dispose(); // Ensure each task's resources are released
            }
            _activeTasks.Clear();
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys.ToList();
        }

        public void Dispose()
        {
            StopAllTasks();
        }
    }

    // Assuming ICPUIntensiveTask interface and CPUIntensiveTask class are defined as below:

    public interface ICPUIntensiveTask : IDisposable
    {
        void Start();
        void Stop();
    }

    public class CPUIntensiveTask : ICPUIntensiveTask
    {
        private int _complexity;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _isRunning = true;
            // Start the task here
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            // Ensure the stopping logic is applied here
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            // Dispose other resources here if any
        }
    }
}