using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceIssues.Services
{
    public class CPUIntensiveTask : ICPUIntensiveTask, IDisposable
    {
        private readonly int _complexity;
        private volatile bool _isRunning;
        private readonly List<double[]> _results = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            _isRunning = true;
            Task.Run(() =>
            {
                CancellationToken token = _cancellationTokenSource.Token;
                try
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
                            if (_results.Count >= 10) // Limit storage to prevent unbounded growth
                            {
                                _results.RemoveAt(0);
                            }
                            _results.Add(results);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Task cancellation logic
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel(); // Signal the task to stop
        }

        public void Dispose()
        {
            Stop(); 
            _cancellationTokenSource.Dispose();
            ClearResults();
        }

        private void ClearResults()
        {
            lock (_results)
            {
                _results.Clear(); // Clear all accumulated data
            }
        }
    }
}