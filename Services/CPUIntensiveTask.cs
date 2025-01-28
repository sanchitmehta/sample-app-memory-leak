namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private bool _isRunning;
        private readonly List<double[]> _results = new();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _isRunning = true;
            _task = Task.Run(() =>
            {
                var token = _cancellationTokenSource.Token;
                while (_isRunning && !token.IsCancellationRequested)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    // Limit the size of _results to prevent memory leaks
                    if (_results.Count >= 100)
                    {
                        _results.Clear();
                    }
                    _results.Add(results);
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _task.Wait();

            // Clear the results to release memory
            _results.Clear();
        }

        public void Dispose()
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _task?.Wait();
            _cancellationTokenSource?.Dispose();
            _task?.Dispose();

            // Clear results to release memory
            _results.Clear();
        }
    }
}