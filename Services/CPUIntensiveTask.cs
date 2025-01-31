namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable // Implement IDisposable for proper cleanup
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private bool _disposed; // To track disposal state
        private readonly object _lock = new(); // Lock to control concurrent access

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));
            }

            _isRunning = true;

            // Use Task.Run and ensure scoped variables are properly managed
            Task.Run(() =>
            {
                while (_isRunning)
                {
                    var results = new double[1000];
                    try
                    {
                        // Perform calculations and write into the buffer
                        for (int i = 0; i < _complexity; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                               Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }

                        // Manage buffer storage to prevent memory bloating
                        lock (_lock)
                        {
                            if (_results.Count >= 100) // Limit size to prevent uncontrolled growth
                            {
                                _results.RemoveAt(0); // Discard the oldest item
                            }
                            _results.Add(results);
                        }
                    }
                    finally
                    {
                        // Explicitly clear the buffer to prevent excessive memory retention
                        Array.Clear(results, 0, results.Length);
                    }
                }
            });
        }

        public void Stop()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));
            }

            _isRunning = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clear all stored results to release memory
                    lock (_lock)
                    {
                        _results.Clear();
                    }
                }
                _disposed = true; // Mark as disposed
            }
        }

        public void Dispose()
        {
            // Public dispose method to ensure proper disposal
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}