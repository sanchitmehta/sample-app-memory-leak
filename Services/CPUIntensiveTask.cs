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
            _results.Clear();
            Task.Run(() =>
            {
                try
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested && _isRunning)
                    {
                        var results = new double[1000];
                        for (int i = 0; i < _complexity && _isRunning; i++)
                        {
                            results[i % 1000] = Math.Pow(Math.Sin(i), Math.Cos(i)) +
                                                Math.Sqrt(Math.Abs(Math.Tan(i)));
                        }

                        lock (_results)
                        {
                            if (_results.Count >= 100) // Avoids unbounded growth
                            {
                                _results.RemoveAt(0); // Remove oldest entry to limit memory usage
                            }
                            _results.Add(results);
                        }
                    }
                }
                catch (OperationCanceledException) { }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _results.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}