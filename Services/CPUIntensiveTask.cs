namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private Task? _task;
        private readonly object _lock = new();

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;
            _task = Task.Run(() =>
            {
                while (_isRunning)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                            Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    lock (_lock)
                    {
                        if (_results.Count >= 100) // Limit size to prevent memory growth
                        {
                            _results.RemoveAt(0); // Remove oldest results
                        }
                        _results.Add(results);
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            _task?.Wait(); // Ensure the task completes before continuing
            _task = null;
        }

        public void Dispose()
        {
            Stop();
            lock (_lock)
            {
                _results.Clear(); // Release memory held by the results
            }
        }
    }
}