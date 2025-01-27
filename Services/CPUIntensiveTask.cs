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
        private List<double[]> _results = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _processingTask;
        private bool _disposed;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));

            _isRunning = true;
            _processingTask = Task.Run(() =>
            {
                try
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested && _isRunning)
                    {
                        var results = new double[1000];
                        for (int i = 0; i < _complexity; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                               Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }

                        lock (_results)
                        {
                            // Limit the size of the results to prevent unbounded growth
                            if (_results.Count >= 10000)
                            {
                                _results.RemoveAt(0);
                            }
                            _results.Add(results);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Operation canceled, exit loop
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _processingTask?.Wait();

            // Clear results to release large object arrays
            lock (_results)
            {
                _results.Clear();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Stop(); // Ensures the task is stopped and memory cleared

            _cancellationTokenSource.Dispose();
            _processingTask?.Dispose();
            _results = null;

            _disposed = true;
        }
    }
}