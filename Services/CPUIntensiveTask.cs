namespace PerformanceIssues.Services
{
    using System.Collections.Concurrent;

    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly ConcurrentQueue<double[]> _results = new(); // Prevent unbounded growth.
        private readonly CancellationTokenSource _cancellationTokenSource = new(); // Proper CancellationToken handling.
        private Task? _task;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;

            // Use CancellationToken for controlled stopping of the Task.
            _task = Task.Run(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested) // Use the cancellation token to stop the task.
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                            Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }

                    // Prevent memory leak by bounding the number of cached results.
                    _results.Enqueue(results);
                    while (_results.Count > 100) // Keep only the last 100 results to prevent unbounded growth.
                    {
                        _results.TryDequeue(out _); // Discard old results.
                    }
                }
            }, _cancellationTokenSource.Token); // Pass the token to the Task.
        }

        public void Stop()
        {
            _isRunning = false;
            // Use cancellation token for thread-safe task completion.
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
            // Wait for the task to complete gracefully.
            if (_task != null)
            {
                Task.WaitAll(_task);
            }
        }

        // Dispose pattern to clean up resources properly.
        public void Dispose()
        {
            Stop(); // Ensure the task is stopped before disposing.
            _cancellationTokenSource.Dispose(); // Dispose of the CancellationTokenSource to release memory.
        }
    }
}