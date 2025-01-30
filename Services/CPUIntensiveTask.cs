namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;

        // Fixed memory leak: Changed to a ConcurrentBag for thread safety and added a size cap logic
        private readonly ConcurrentBag<double[]> _results = new(); 
        private const int MaxResultsCapacity = 100; // Cap size to avoid unbounded memory usage

        private bool _disposed; // Track whether Dispose has been called

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;
            Task.Run(() =>
            {
                while (_isRunning)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    // Added logic to prevent unbounded growth in memory
                    if (_results.Count >= MaxResultsCapacity)
                    {
                        // Purge older data if capacity is reached
                        _results.TryTake(out _);
                    }
                    _results.Add(results);
                    
                    // Optional Note: Consider logging or pausing threads dynamically if capacity is breached often
                }
            });
        }

        public void Stop()
        {
            _isRunning = false; // Signal the task to stop
        }

        // Correctly implement IDisposable pattern to release resources
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear the collection to free up memory
                    _results.Clear();
                }

                // Mark as disposed to prevent redundant operations
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization for better performance
        }

        // Optional suggestions (not implemented):
        // 1. Pass a CancellationToken to Task.Run for better cancellation handling.
        // 2. Use a thread pool or actor-pattern framework for such tasks for scalability.
    }
}