using System.Collections.Concurrent;

namespace PerformanceIssues.Services // Fixed typo in namespace ("Serivces" → "Services")
{
    public class CPUTaskManager : IDisposable // Implement IDisposable to ensure proper cleanup of resources
    {
        private readonly ConcurrentDictionary<string, ICPUIntensiveTask> _activeTasks = new();
        private bool _disposed; // To track whether Dispose has been called

        public string StartNewTask(int complexity)
        {
            var taskId = Guid.NewGuid().ToString();
            var task = new CPUIntensiveTask(complexity);

            // Ensure proper disposal of CPUIntensiveTask
            task.Start();
            _activeTasks.TryAdd(taskId, task);
            return taskId;
        }

        public bool StopTask(string taskId)
        {
            if (_activeTasks.TryRemove(taskId, out var task))
            {
                task.Stop();

                // Ensure task is disposed after stopping
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

                // Dispose of each task to release resources
                if (task is IDisposable disposableTask)
                {
                    disposableTask.Dispose();
                }
            }
            _activeTasks.Clear(); // Clear the dictionary to release references
        }

        public IEnumerable<string> GetActiveTasks()
        {
            return _activeTasks.Keys;
        }

        // Implement the Dispose method following the disposal pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose of active tasks
                StopAllTasks();

                // Dispose of any other disposables here if needed
                // (e.g., other managed resources)
            }

            _disposed = true;
        }

        ~CPUTaskManager()
        {
            Dispose(false);
        }
    }
}