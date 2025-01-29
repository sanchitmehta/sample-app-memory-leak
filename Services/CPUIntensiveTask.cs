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
        private CancellationTokenSource _cancellationTokenSource;

        public CPUIntensiveTask(int complexity)
        {
            _complexity = complexity;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _isRunning = true;
            Task.Run(() =>
            {
                try
                {
                    while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var results = new double[1000];
                        for (int i = 0; i < _complexity; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                                Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }
                        lock (_results)
                        {
                            if (_results.Count > 100) // Avoid unbounded growth
                            {
                                _results.RemoveAt(0);
                            }
                            _results.Add(results);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Gracefully handle cancellation
                }
                finally
                {
                    ClearResults();
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
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
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            ClearResults();
        }
    }
}