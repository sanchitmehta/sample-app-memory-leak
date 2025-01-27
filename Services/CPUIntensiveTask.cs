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
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            _isRunning = true;
            Task.Run(() =>
            {
                while (_isRunning && !token.IsCancellationRequested)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    lock (_results)
                    {
                        // Memory leak fixed by capping the list size
                        if (_results.Count >= 100)
                        {
                            _results.RemoveAt(0);
                        }
                        _results.Add(results);
                    }
                }
            }, token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            lock (_results)
            {
                _results.Clear();
            }
        }
    }
}