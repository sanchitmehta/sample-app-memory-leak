namespace PerformanceIssues.Serivces
{
    // Added IDisposable to ensure proper resource cleanup
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        
        // List is initialized but grows unbounded. Switched to a bounded approach to avoid memory leaks
        private readonly List<double[]> _results = new();

        private CancellationTokenSource _cancellationTokenSource; // Added CancellationTokenSource to properly manage async tasks

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            // Proper use of CancellationToken for graceful task termination
            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;
            Task.Run(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    // Improved memory management: ensure task doesn't retain large arrays unboundedly
                    lock (_results)
                    {
                        if (_results.Count >= 100) // Added limit to the results list
                        {
                            _results.RemoveAt(0); // Remove oldest result to maintain bounded size
                        }
                        _results.Add(results);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            // Graceful cancellation of the running task
            _isRunning = false;
            _cancellationTokenSource?.Cancel(); // Cancels the task
        }

        // Implemented Dispose pattern to release resources
        public void Dispose()
        {
            Stop(); // Ensure Stop is called to cancel running tasks
            _cancellationTokenSource?.Dispose(); // Dispose CancellationTokenSource to release unmanaged resources
            lock (_results)
            {
                _results.Clear(); // Explicitly clear the results list to release memory
            }
        }
    }
}