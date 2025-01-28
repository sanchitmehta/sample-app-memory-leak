namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private CancellationTokenSource _cancellationTokenSource;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _isRunning = true;
            Task.Run(async () =>
            {
                var cancellationToken = _cancellationTokenSource.Token;
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    if (_results.Count < 100) // Limit the size to prevent indefinite growth
                    {
                        _results.Add(results);
                    }
                    await Task.Delay(100, cancellationToken); // Allow for a small wait to simulate work being done
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _results.Clear(); // Release resources held by the results
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource.Dispose();
        }
    }
}