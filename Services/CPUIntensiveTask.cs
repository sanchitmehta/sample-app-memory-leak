namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private Task _task;

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
                    // Bound the size of the results to prevent memory leak
                    if (_results.Count > 100)
                    {
                        _results.RemoveAt(0);
                    }
                    _results.Add(results);
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            _task?.Wait();
        }

        public void Dispose()
        {
            Stop();
            _results.Clear();
            _task?.Dispose();
        }
    }
}