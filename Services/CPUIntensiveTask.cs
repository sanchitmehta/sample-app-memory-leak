namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private bool _isRunning;
        private readonly List<double[]> _results = new();
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;
            var cancellationToken = _cancellationTokenSource.Token;
            _task = Task.Run(() =>
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    // Limit the size of _results to avoid memory leak
                    lock (_results)
                    {
                        if (_results.Count >= 100)
                        {
                            _results.Clear();
                        }
                        _results.Add(results);
                    }
                }
            }, cancellationToken);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            if (_task != null)
            {
                try
                {
                    _task.Wait();
                }
                catch (AggregateException) { }
                finally
                {
                    _task.Dispose();
                }
            }
            ClearResults();
        }

        private void ClearResults()
        {
            lock (_results)
            {
                _results.Clear();
            }
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource.Dispose();
            ClearResults();
        }
    }
}