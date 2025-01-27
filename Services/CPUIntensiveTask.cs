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
        private readonly CancellationTokenSource _cts = new();
        private Task? _backgroundTask;
        private bool _disposed;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
        }

        public void Start()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CPUIntensiveTask));
            }

            _isRunning = true;
            _backgroundTask = Task.Run(() =>
            {
                while (!_cts.Token.IsCancellationRequested && _isRunning)
                {
                    var results = new double[1000];
                    for (int i = 0; i < _complexity; i++)
                    {
                        results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                          Math.Sqrt(Math.Abs(Math.Tan(i)));
                    }
                    lock (_results)
                    {
                        if (_isRunning) // Double-check to avoid unnecessary additions during shutdown
                        {
                            _results.Add(results);
                        }
                    }
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            _backgroundTask?.Wait();
            lock (_results)
            {
                _results.Clear(); // Release memory held by large collections
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop(); // Ensure the task is stopped and memory is cleared
                _cts.Dispose(); // Dispose of the CancellationTokenSource
                _backgroundTask?.Dispose(); // Dispose of any background task
                _disposed = true;
            }
        }
    }
}