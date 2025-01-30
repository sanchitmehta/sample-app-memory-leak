namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable // Implement IDisposable for cleanup
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private readonly object _lock = new(); // Lock object to manage thread safety for _results
        private Task? _task; // Task needs proper disposal

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_isRunning) return; // Prevent starting multiple tasks
            _isRunning = true;
            _task = Task.Run(() =>
            {
                while (_isRunning)
                {
                    double[] results = null!;
                    try
                    {
                        results = new double[1000];
                        for (int i = 0; i < _complexity; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                              Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }

                        // Potential memory issue: Unbounded growth
                        lock (_lock) // Use lock for thread safety
                        {
                            if (_results.Count > 1000) // Introduce a size limit
                            {
                                _results.RemoveAt(0); // Maintain size by removing old entries
                            }
                            _results.Add(results);
                        }
                    }
                    catch
                    {
                        results = null; // Ensure memory is released in exception cases
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            if (_task != null)
            {
                _task.Wait(); // Ensure task completion before cleanup
                _task.Dispose(); // Properly dispose the task
                _task = null; // Nullify to prevent further use
            }
        }

        public void Dispose() // Implement the Dispose pattern for releasing resources
        {
            Stop(); // Ensure operations are stopped before disposal
            lock (_lock)
            {
                _results.Clear(); // Explicitly clear the collection to free memory
            }
        }
    }
}